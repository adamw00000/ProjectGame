using ConnectionLib;
using System;
using System.Threading.Tasks;

namespace GameLib
{
    public class GameMaster
    {
        private readonly IConnection connection;
        private readonly IGameMasterMessageFactory messageFactory;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private GameMasterState state;
        private readonly GameRules rules;
        private DateTime start;
        public bool gameStarted = false;

        public GameMasterStateSnapshot GameMasterStateSnapshot
        {
            get
            {
                return new GameMasterStateSnapshot(state);
            }

        }

        public GameMaster(GameRules rules, IConnection connection)
        public GameMaster(GameRules rules, IConnection connection, IGameMasterMessageFactory messageFactory)
        {
            this.connection = connection;
            this.messageFactory = messageFactory;
            this.rules = rules;
            state = new GameMasterState(rules);
            logger.Trace("Game Master Created");
        }

        public void JoinGame(int agentId, int teamId, bool wantToBeLeader)
        {
            Message response;
            logger.Debug($"Agent {agentId} wants to join the game as {((Team)teamId).ToString()} and {(wantToBeLeader ? "wants" : "does not want")} to be leader");
            try
            {
                state.JoinGame(agentId, teamId, wantToBeLeader);
                response = messageFactory.CreateJoinGameResponseMessage(agentId, true);
                logger.Debug($"Agent {agentId} joined the game");
            }
            catch (GameSetupException e)
            {
                response = messageFactory.CreateJoinGameResponseMessage(agentId, false);
                logger.Debug(e, $"Agent {agentId} didn't join the game :");
            }
            connection.Send(response);
            
            if (state.PlayerStates.Count == rules.TeamSize * 2)
            {
                logger.Info("Both teams are full - preparing to start the game");
                start = DateTime.UtcNow;
                state.InitializePlayerPositions(rules.BoardWidth, rules.BoardHeight, rules.TeamSize);
                var rulesDict = state.GetAgentGameRules();
                gameStarted = true;
                logger.Debug("Game started - sending messages...");
                logger.Debug($"Game rules are \n{rules.ToString()}");
                foreach (var (playerId, state) in state.PlayerStates)
                {
                    logger.Debug($"Agent {playerId} is in {state.Team}{(state.IsLeader ? " and is leader}" : "")}");
                    Message startGameMessage = messageFactory.CreateGameStartMessage(playerId, (long)(start - new DateTime()).TotalMilliseconds, rulesDict[playerId]);
                    connection.Send(startGameMessage);
                }
                logger.Info("Game started");
            }
        }

        public async Task GeneratePieces()
        {
            state.GeneratePiece();
            /*while (!state.GameEnded)
            {
                state.GeneratePiece();
                await Task.Delay(rules.PieceSpawnInterval).ConfigureAwait(false);
            }*/
        }

        public async Task ListenJoiningAndStart()
        {
            logger.Info("Waiting for agents...");
            while (!gameStarted)
            {
                Message message = await connection.ReceiveAsync();
                message.Handle(this);
            }
            StartGame();
        }

        public async Task StartGame()
        {
            try
            {
                while (!state.GameEnded)
                {
                    logger.Trace("Starting pieces generating");
                    await GeneratePieces();
                    DateTime nextPieceGeneratingTime = DateTime.UtcNow.AddMilliseconds(rules.PieceSpawnInterval);
                    while (DateTime.UtcNow < nextPieceGeneratingTime)
                    {
                        int timespan = Math.Max(0, (int)(nextPieceGeneratingTime - DateTime.UtcNow).TotalMilliseconds);
                        if (connection.TryReceive(out Message message, timespan))
                        {
                            logger.Trace(message.ToString() + " handling started");
                            message.Handle(this);
                            logger.Trace(message.ToString() + " handling finished");
                        }
                    }
                }
                logger.Info($"Game ended - the winner is {state.Winner.ToString()}");
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        private (int timestamp, int waitUntil) CalculateDelay(int agentId)
        {
            PlayerState agentPlayerState = state.PlayerStates[agentId];
            DateTime waitUntil = agentPlayerState.LastRequestTimestamp.AddMilliseconds(agentPlayerState.LastActionDelay);
            (int timestamp, int waitUntil) result = (CurrentTimestamp(), (int)(waitUntil - start).TotalMilliseconds);
            logger.Trace($"Calculation for agent {agentId} - timestamp {result.timestamp}, waitUntil {result.waitUntil}");
            return result;
        }

        private int CurrentTimestamp()
        {
            return (int)(DateTime.UtcNow - start).TotalMilliseconds;
        }
        public void MoveAgent(int agentId, MoveDirection moveDirection, string messageId)
        {
            logger.Debug($"Agent {agentId} wants to move {moveDirection.ToString()}");
            Message response;
            try
            {
                int distance = state.Move(agentId, moveDirection);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug($"Agent {agentId} moved {moveDirection.ToString()}");
                response = messageFactory.CreateMakeMoveResponseMessage(agentId, timestamp, waitUntil, distance, messageId);
            }
            catch (PendingLeaderCommunicationException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't move: ");
                response = messageFactory.CreateInvalidActionErrorMessage(agentId, CurrentTimestamp(), messageId);
            }
            catch (DelayException e)
            {
                logger.Warn(e, $"Agent {agentId} tried move during penalty: ");
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = messageFactory.CreateTimePenaltyErrorMessage(agentId, timestamp, waitUntil, messageId);
            }
            catch (InvalidMoveException e)
            {
                logger.Warn(e, $"Agent {agentId} couldn't move: ");
                response = messageFactory.CreateInvalidMoveDirectionErrorMessage(agentId, CurrentTimestamp(), messageId);
            }
            connection.Send(response);
        }

        public void PickPiece(int agentId, string messageId)
        {
            logger.Debug($"Agent {agentId} wants to pick up a piece");
            Message response;
            try
            {
                state.PickUpPiece(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug($"Agent {agentId} picked up a piece");
                response = messageFactory.CreatePickPieceResponseMessage(agentId, timestamp, waitUntil, messageId);
            }
            catch (PendingLeaderCommunicationException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't pick up a piece: ");
                response = messageFactory.CreateInvalidActionErrorMessage(agentId, CurrentTimestamp(), messageId);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't pick up a piece: ");
                response = messageFactory.CreateTimePenaltyErrorMessage(agentId, timestamp, waitUntil, messageId);
            }
            catch (PieceOperationException e)
            {
                logger.Warn(e, $"Agent {agentId} couldn't pick up a piece: ");
                response = messageFactory.CreateInvalidActionErrorMessage(agentId, CurrentTimestamp(), messageId);
            }
            connection.Send(response);
        }

        public void PutPiece(int agentId, string messageId)
        {
            logger.Debug($"Agent {agentId} wants to put a piece");
            Message response;
            try
            {
                PutPieceResult result = state.PutPiece(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug($"Agent {agentId} put a piece");
                response = messageFactory.CreatePutPieceResponseMessage(agentId, timestamp, waitUntil, result, messageId);
            }
            catch (PendingLeaderCommunicationException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't put a piece: ");
                response = messageFactory.CreateInvalidActionErrorMessage(agentId, CurrentTimestamp(), messageId);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug(e, $"Agent {agentId} couldn't put a piece: ");
                response = messageFactory.CreateTimePenaltyErrorMessage(agentId, timestamp, waitUntil, messageId);
            }
            catch (PieceOperationException e)
            {
                logger.Debug(e, $"Agent {agentId} couldn't put a piece: ");
                response = messageFactory.CreateInvalidActionErrorMessage(agentId, CurrentTimestamp(), messageId);
            }
            connection.Send(response);
            if (state.GameEnded)
            {
                logger.Debug($"Last piece was put - ending game");
                int timestamp = CurrentTimestamp();
                foreach (var (id, agplayerId) in state.PlayerStates)
                {
                    Message message = messageFactory.CreateGameOverMessage(id, timestamp, state.Winner.Value);
                    connection.Send(message);
                }
            }
        }

        public void Discover(int agentId, string messageId)
        {
            logger.Debug($"Agent {agentId} wants to discover nearby pieces");
            Message response;
            try
            {
                DiscoveryResult discoveryResult = state.Discover(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug($"Agent {agentId} made action discovery successfully");
                response = messageFactory.CreateDiscoveryResponseMessage(agentId, timestamp, waitUntil, discoveryResult, messageId);
            }
            catch (PendingLeaderCommunicationException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't make discovery action: ");
                response = messageFactory.CreateInvalidActionErrorMessage(agentId, CurrentTimestamp(), messageId);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't make discovery action: ");
                response = messageFactory.CreateTimePenaltyErrorMessage(agentId, timestamp, waitUntil, messageId);
            }
            connection.Send(response);
        }

        public void CheckPiece(int agentId, string messageId)
        {
            logger.Debug($"Agent {agentId} wants to check piece");
            Message response;
            try
            {
                bool result = state.CheckPiece(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug($"Agent {agentId} checked his piece - it's {(result ? "valid" : "not valid")}");
                response = messageFactory.CreateCheckPieceResponseMessage(agentId, timestamp, waitUntil, result, messageId);
            }
            catch (PendingLeaderCommunicationException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't check his piece: ");
                response = messageFactory.CreateInvalidActionErrorMessage(agentId, CurrentTimestamp(), messageId);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't check his piece: ");
                response = messageFactory.CreateTimePenaltyErrorMessage(agentId, timestamp, waitUntil, messageId);
            }
            catch (PieceOperationException e)
            {
                logger.Warn(e, $"Agent {agentId} couldn't check his piece: ");
                response = messageFactory.CreateInvalidActionErrorMessage(agentId, CurrentTimestamp(), messageId);
            }
            connection.Send(response);
        }

        public void DestroyPiece(int agentId, string messageId)
        {
            logger.Debug($"Agent {agentId} wants to destroy piece");
            Message response;
            try
            {
                state.DestroyPiece(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug($"Agent {agentId} destroyed piece");
                response = messageFactory.CreateDestoryPieceResponseMessage(agentId, timestamp, waitUntil, messageId); 
            }
            catch (PendingLeaderCommunicationException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't destroy piece: ");
                response = messageFactory.CreateInvalidActionErrorMessage(agentId, CurrentTimestamp(), messageId);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't destroy piece: ");
                response = messageFactory.CreateTimePenaltyErrorMessage(agentId, timestamp, waitUntil, messageId);
            }
            catch (PieceOperationException e)
            {
                logger.Warn(e, $"Agent {agentId} couldn't destroy piece: ");
                response = messageFactory.CreateInvalidActionErrorMessage(agentId, CurrentTimestamp(), messageId);
            }
            connection.Send(response);
        }

        public void CommunicationRequestWithData(int requesterAgentId, int targetAgentId, object data, string messageId)
        {
            logger.Debug($"Agent {requesterAgentId} wants to communicate with {targetAgentId} and data {data.ToString()}");
            try
            {
                state.SaveCommunicationData(requesterAgentId, targetAgentId, data, messageId);
                logger.Debug($"Agent {requesterAgentId} successfully requested agent {targetAgentId} to communicate with data {data.ToString()}");
                Message request = messageFactory.CreateCommunicationRequestMessage(requesterAgentId, targetAgentId, CurrentTimestamp());
                connection.Send(request);
            }
            catch (PendingLeaderCommunicationException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(requesterAgentId);
                logger.Warn(e, $"Agent {requesterAgentId} couldn't request communication with {targetAgentId} and data {data.ToString()}: ");
                Message response = messageFactory.CreateInvalidActionErrorMessage(requesterAgentId, CurrentTimestamp(), messageId);
                connection.Send(response);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(requesterAgentId);
                logger.Warn(e, $"Agent {requesterAgentId} couldn't request communication with {targetAgentId} and data {data.ToString()}");
                Message response = messageFactory.CreateTimePenaltyErrorMessage(requesterAgentId, timestamp, waitUntil, messageId);
                connection.Send(response);
            }
        }

        public void CommunicationAgreementWithData(int requesterAgentId, int targetAgentId, bool agreement, object targetData, string targetMessageId)
        {
            logger.Debug($"Agent {requesterAgentId} was tried to be answered by {targetAgentId} and data {(targetData == null ? "null" : targetData.ToString())}");
            try
            {
                state.VerifyLeaderCommunicationState(requesterAgentId, targetAgentId, agreement);
            }
            catch (PendingLeaderCommunicationException e)
            {
                int timestamp = CurrentTimestamp();
                logger.Warn(e, $"Error during proccesing answer from {targetAgentId} to {requesterAgentId} with data {(targetData == null ? "null" : targetData.ToString())} (pending leader communication): ");
                Message response = messageFactory.CreateInvalidActionErrorMessage(targetAgentId, timestamp, targetMessageId);
                connection.Send(response);
                return;
            }

            if (targetData == null)
            {
                logger.Warn($"Agent {targetAgentId} sent null as targetData");
                Message response = messageFactory.CreateInvalidActionErrorMessage(targetAgentId, CurrentTimestamp(), targetMessageId);
                connection.Send(response);
                return;
            }

            (object data, string senderMessageId) senderData;
            try
            {
                senderData = state.GetCommunicationData(requesterAgentId, targetAgentId); //check if communication exists and get its data
            }
            catch (CommunicationException e) //if communication data does not exist
            {
                int timestamp = CurrentTimestamp();
                logger.Warn(e, $"Error during proccesing answer from {targetAgentId} to {requesterAgentId} with data {targetData.ToString()}: ");
                Message response = messageFactory.CreateInvalidActionErrorMessage(targetAgentId, timestamp, targetMessageId);
                connection.Send(response);
                return;
            }

            if (!agreement)
            {
                int timestamp = CurrentTimestamp();
                logger.Debug($"Request of agent {requesterAgentId} was rejected by {targetAgentId}");
                Message response = messageFactory.CreateCommunicationResponseWithDataMessage(requesterAgentId, timestamp, 
                    CalculateDelay(requesterAgentId).waitUntil, targetAgentId, false, null, senderData.senderMessageId);
                connection.Send(response);
                return;
            }

            state.DelayCommunicationPartners(requesterAgentId, targetAgentId);

            Message responseToSender, responseToTarget;
            (int timestamp1, int waitUntil1) = CalculateDelay(requesterAgentId);
            (int timestamp2, int waitUntil2) = CalculateDelay(targetAgentId);
            logger.Debug($"Agent {requesterAgentId} was answered by {targetAgentId} and data {(targetData.ToString())}");
            responseToSender = messageFactory.CreateCommunicationResponseWithDataMessage(requesterAgentId, timestamp1, waitUntil1, targetAgentId, true, targetData, senderData.senderMessageId);
            responseToTarget = messageFactory.CreateCommunicationResponseWithDataMessage(targetAgentId, timestamp2, waitUntil2, requesterAgentId, true, senderData.data, targetMessageId);

            connection.Send(responseToSender);
            connection.Send(responseToTarget);

        }
    }
}