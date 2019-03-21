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
        }

        public void GenerateBoard()
        {
            state = new GameMasterState(rules);
        }

        public void JoinGame(int agentId, int teamId, bool wantToBeLeader)
        {
            Message response;
            try
            {
                state.JoinGame(agentId, teamId, wantToBeLeader);
                response = new JoinGameResponseMessage(agentId, teamId, true);
            }
            catch(GameSetupException e)
            {
                response = new JoinGameResponseMessage(agentId, teamId, false);
            }
            connection.Send(response);
            if(state.PlayerStates.Count == rules.TeamSize * 2)
            {
                start = DateTime.UtcNow;
                state.InitializePlayerPositions(rules.BoardWidth, rules.BoardHeight, rules.TeamSize);
                var rulesDict = state.GetAgentGameRules();
                gameStarted = true;
                foreach (var (playerId, state) in state.PlayerStates)
                {
                    Message startGameMessage = new GameStartMessage(playerId, 0, rulesDict[playerId]); //0 stands for start of the game
                    connection.Send(startGameMessage);
                }
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
            while(!gameStarted)
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
                    await GeneratePieces();
                    DateTime nextPieceGeneratingTime = DateTime.UtcNow.AddMilliseconds(rules.PieceSpawnInterval);
                    while (DateTime.UtcNow < nextPieceGeneratingTime)
                    {
                        int timespan = Math.Max(0, (int)(nextPieceGeneratingTime - DateTime.UtcNow).TotalMilliseconds);
                        if (connection.TryReceive(out Message message, timespan))
                        {
                            //Console.WriteLine(message.ToString() + " started");
                            message.Handle(this);
                            //Console.WriteLine(message.ToString() + " ended");
                        }
                    }
                }
                Console.WriteLine("Game ended");
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
            return ((int)(agentPlayerState.LastRequestTimestamp - start).TotalMilliseconds, (int)(waitUntil - start).TotalMilliseconds);
        }

        private int ClosestPieceDistance(int agentId)
        {
            PlayerState agentPlayerState = state.PlayerStates[agentId];
            return state.Board[agentPlayerState.Position.X, agentPlayerState.Position.Y].Distance;
        }

        private int CurrentTimestamp()
        {
            return (int)(DateTime.UtcNow - start).TotalMilliseconds;
        }
        public void MoveAgent(int agentId, MoveDirection moveDirection)
        {
            Message response;
            try
            {
                state.Move(agentId, moveDirection);
                int distance = ClosestPieceDistance(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = new ActionMakeMoveResponse(agentId, timestamp, waitUntil, distance);
            }
            catch(DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch(InvalidMoveException e)
            {
                response = new InvalidMoveDirectionError(agentId, CurrentTimestamp());
            }
            connection.Send(response);
        }

        public void PickPiece(int agentId)
        {
            Message response;
            try
            {
                state.PickUpPiece(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = new ActionPickPieceResponse(agentId, timestamp, waitUntil);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch (PieceOperationException e)
            {
                response = new InvalidAction(agentId, CurrentTimestamp());
            }
            connection.Send(response);
        }

        public void PutPiece(int agentId)
        {
            Message response;
            try
            {
                PutPieceResult result = state.PutPiece(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = new ActionPutPieceResponse(agentId, timestamp, waitUntil, result);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch (PieceOperationException e)
            {
                response = new InvalidAction(agentId, CurrentTimestamp());
            }
            connection.Send(response);
            if(state.GameEnded)
            {
                int timestamp = CurrentTimestamp();
                foreach(var (id, agplayerId) in state.PlayerStates)
                {
                    Message message = new GameOverMessage(id, timestamp, (int) state.Winner);
                }
            }
        }

        public void Discover(int agentId)
        {
            Message response;
            try
            {
                DiscoveryResult discoveryResult = state.Discover(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = new ActionDiscoverResponse(agentId, timestamp, waitUntil, discoveryResult);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            connection.Send(response);
        }

        public void CheckPiece(int agentId)
        {
            Message response;
            try
            {
                bool result = state.CheckPiece(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = new ActionCheckPieceResponse(agentId, timestamp, waitUntil, result);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch (PieceOperationException e)
            {
                response = new InvalidAction(agentId, CurrentTimestamp());
            }
            connection.Send(response);
        }

        public void DestroyPiece(int agentId)
        {
            Message response;
            try
            {
                state.DestroyPiece(agentId);
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = new ActionDestroyPieceResponse(agentId, timestamp, waitUntil);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(agentId);
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch (PieceOperationException e)
            {
                response = new InvalidAction(agentId, CurrentTimestamp());
            }
            connection.Send(response);
        }

        public void CommunicationRequestWithData(int requesterAgentId, int targetAgentId, Object data)
        {
            try
            {
                state.SaveCommunicationData(requesterAgentId, targetAgentId, data);
                Message request = new ActionCommunicationRequest(requesterAgentId, targetAgentId, CurrentTimestamp());
                connection.Send(request);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = CalculateDelay(requesterAgentId);
                Message response = new RequestTimePenaltyError(requesterAgentId, timestamp, waitUntil);
                connection.Send(response);
            }
        }

        public void CommunicationAgreementWithData(int requesterAgentId, int targetAgentId, bool agreement, Object data)
        {
            if(agreement == false)
            {
                int timestamp = CurrentTimestamp();
                Message response = new ActionCommunicationResponseWithData(requesterAgentId, timestamp, timestamp, targetAgentId, false, null);
                connection.Send(response);
            }
            else
            {
                if (state.GetCommunicationData(requesterAgentId, targetAgentId) == null)
                {
                    throw new NotImplementedException();
                }
                Message responseToSender, responseToTarget;
                state.DelayCommunicationPartners(requesterAgentId, targetAgentId);
                (int timestamp1, int waitUntil1) = CalculateDelay(requesterAgentId);
                (int timestamp2, int waitUntil2) = CalculateDelay(targetAgentId);
                responseToSender = new ActionCommunicationResponseWithData(requesterAgentId, timestamp1, waitUntil1, targetAgentId, true, data);
                responseToTarget = new ActionCommunicationResponseWithData(targetAgentId, timestamp2, waitUntil2, requesterAgentId, true, state.GetCommunicationData(requesterAgentId,targetAgentId));
                state.SaveCommunicationData(requesterAgentId, targetAgentId, null);
                //Needs to be moved to state ^
                connection.Send(responseToSender);
                connection.Send(responseToTarget);
            }
        }
    }
}