using ConnectionLib;
using GameLib.Actions;
using GameLib.GameMessages;
using System;
using System.Threading.Tasks;

namespace GameLib
{
    public class GameMaster
    {
        private readonly IConnection connection;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public GameMasterState state; //GUI needs it
        private readonly GameRules rules;
        private DateTime start;
        public bool gameStarted = false;

        public GameMaster(GameRules rules, IConnection connection)
        {
            this.connection = connection;
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
                response = new JoinGameResponseMessage(agentId, teamId, true);
                logger.Debug($"Agent {agentId} joined the game");
            }
            catch (GameSetupException e)
            {
                response = new JoinGameResponseMessage(agentId, teamId, false);
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
                    Message startGameMessage = new GameStartMessage(playerId, 0, rulesDict[playerId], 
                        (long)(start - new DateTime()).TotalMilliseconds); //0 stands for start of the game
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
        public void MoveAgent(int agentId, MoveDirection moveDirection)
        {
            logger.Debug($"Agent {agentId} wants to move {moveDirection.ToString()}");
            Message response;
            try
            {
                int distance = state.Move(agentId, moveDirection);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug($"Agent {agentId} moved {moveDirection.ToString()}");
                response = new ActionMakeMoveResponse(agentId, timestamp, waitUntil, distance);
            }
            catch (DelayException e)
            {
                logger.Warn(e, $"Agent {agentId} tried move during penalty: ");
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch (InvalidMoveException e)
            {
                logger.Warn(e, $"Agent {agentId} couldn't move: ");
                response = new InvalidMoveDirectionError(agentId, CurrentTimestamp());
            }
            connection.Send(response);
        }

        public void PickPiece(int agentId)
        {
            logger.Debug($"Agent {agentId} wants to pick up a piece");
            Message response;
            try
            {
                state.PickUpPiece(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug($"Agent {agentId} picked up a piece");
                response = new ActionPickPieceResponse(agentId, timestamp, waitUntil);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't pick up a piece: ");
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch (PieceOperationException e)
            {
                logger.Warn(e, $"Agent {agentId} couldn't pick up a piece: ");
                response = new InvalidAction(agentId, CurrentTimestamp());
            }
            connection.Send(response);
        }

        public void PutPiece(int agentId)
        {
            logger.Debug($"Agent {agentId} wants to put a piece");
            Message response;
            try
            {
                PutPieceResult result = state.PutPiece(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug($"Agent {agentId} put a piece");
                response = new ActionPutPieceResponse(agentId, timestamp, waitUntil, result);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug(e, $"Agent {agentId} couldn't put a piece: ");
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch (PieceOperationException e)
            {
                logger.Debug(e, $"Agent {agentId} couldn't put a piece: ");
                response = new InvalidAction(agentId, CurrentTimestamp());
            }
            connection.Send(response);
            if (state.GameEnded)
            {
                logger.Debug($"Last piece was put - ending game");
                int timestamp = CurrentTimestamp();
                foreach (var (id, agplayerId) in state.PlayerStates)
                {
                    Message message = new GameOverMessage(id, timestamp, (int)state.Winner);
                    connection.Send(message);
                }
            }
        }

        public void Discover(int agentId)
        {
            logger.Debug($"Agent {agentId} wants to discover nearby pieces");
            Message response;
            try
            {
                DiscoveryResult discoveryResult = state.Discover(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug($"Agent {agentId} made action discovery successfully");
                response = new ActionDiscoverResponse(agentId, timestamp, waitUntil, discoveryResult);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't make discovery action: ");
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            connection.Send(response);
        }

        public void CheckPiece(int agentId)
        {
            logger.Debug($"Agent {agentId} wants to check piece");
            Message response;
            try
            {
                bool result = state.CheckPiece(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug($"Agent {agentId} checked his piece - it's {(result ? "valid" : "not valid")}");
                response = new ActionCheckPieceResponse(agentId, timestamp, waitUntil, result);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't check his piece: ");
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch (PieceOperationException e)
            {
                logger.Warn(e, $"Agent {agentId} couldn't check his piece: ");
                response = new InvalidAction(agentId, CurrentTimestamp());
            }
            connection.Send(response);
        }

        public void DestroyPiece(int agentId)
        {
            logger.Debug($"Agent {agentId} wants to destroy piece");
            Message response;
            try
            {
                state.DestroyPiece(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Debug($"Agent {agentId} destroyed piece");
                response = new ActionDestroyPieceResponse(agentId, timestamp, waitUntil);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                logger.Warn(e, $"Agent {agentId} couldn't destroy piece: ");
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch (PieceOperationException e)
            {
                logger.Warn(e, $"Agent {agentId} couldn't destroy piece: ");
                response = new InvalidAction(agentId, CurrentTimestamp());
            }
            connection.Send(response);
        }

        public void CommunicationRequestWithData(int requesterAgentId, int targetAgentId, object data)
        {
            logger.Debug($"Agent {requesterAgentId} wants to communicate with {targetAgentId} and data {data.ToString()}");
            try
            {
                state.SaveCommunicationData(requesterAgentId, targetAgentId, data);
                logger.Debug($"Agent {requesterAgentId} successfully requested agent {targetAgentId} to communicate with data {data.ToString()}");
                Message request = new ActionCommunicationRequest(requesterAgentId, targetAgentId, CurrentTimestamp());
                connection.Send(request);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(requesterAgentId);
                logger.Warn(e, $"Agent {requesterAgentId} couldn't request communication with {targetAgentId} and data {data.ToString()}");
                Message response = new RequestTimePenaltyError(requesterAgentId, timestamp, waitUntil);
                connection.Send(response);
            }
        }

        public void CommunicationAgreementWithData(int requesterAgentId, int targetAgentId, bool agreement, object targetData)
        {
            logger.Debug($"Agent {requesterAgentId} was tried to be answered by {targetAgentId} and data {(targetData == null ? "null" : targetData.ToString())}");
            if (targetData == null)
            {
                logger.Warn($"Agent {targetAgentId} sent null as targetData");
                Message response = new InvalidAction(targetAgentId, CurrentTimestamp());
                connection.Send(response);
                return;
            }
            object senderData = null;
            try
            {
                senderData = state.GetCommunicationData(requesterAgentId, targetAgentId); //check if communication exists and get its data
            }
            catch (CommunicationException e) //if communication data does not exist
            {
                int timestamp = CurrentTimestamp();
                logger.Warn(e, $"Error during proccesing answer from {targetAgentId} to {requesterAgentId} with data {targetData.ToString()}: ");
                Message response = new InvalidAction(targetAgentId, timestamp);
                connection.Send(response);
                return;
            }

            if (!agreement)
            {
                int timestamp = CurrentTimestamp();
                logger.Debug($"Request of agent {requesterAgentId} was rejected by {targetAgentId}");
                Message response = new ActionCommunicationResponseWithData(requesterAgentId, timestamp, 
                    CalculateDelay(requesterAgentId).waitUntil, targetAgentId, false, null);
                connection.Send(response);
                return;
            }

            state.DelayCommunicationPartners(requesterAgentId, targetAgentId);

            Message responseToSender, responseToTarget;
            (int timestamp1, int waitUntil1) = CalculateDelay(requesterAgentId);
            (int timestamp2, int waitUntil2) = CalculateDelay(targetAgentId);
            logger.Debug($"Agent {requesterAgentId} was answered by {targetAgentId} and data {(targetData.ToString())}");
            responseToSender = new ActionCommunicationResponseWithData(requesterAgentId, timestamp1, waitUntil1, targetAgentId, true, targetData);
            responseToTarget = new ActionCommunicationResponseWithData(targetAgentId, timestamp2, waitUntil2, requesterAgentId, true, senderData);

            connection.Send(responseToSender);
            connection.Send(responseToTarget);

        }
    }
}