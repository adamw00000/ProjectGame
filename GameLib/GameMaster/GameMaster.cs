using ConnectionLib;
using GameLib.Actions;
using GameLib.GameMessages.GameSetup;
using System;
using System.Threading.Tasks;

namespace GameLib
{
    public class GameMaster
    {
        private readonly IConnection connection;

        private GameMasterState state;
        private readonly GameRules rules;
        private DateTime start;

        public GameMaster(GameRules rules, IConnection connection)
        {
            this.connection = connection;
            this.rules = rules;
        }

        public void GenerateBoard()
        {
            state = new GameMasterState(rules);
        }

        public void JoinToGame(int agentId, int teamId, bool wantToBeLeader)
        {
            Message response;
            try
            {
                state.JoinGame(agentId, teamId, wantToBeLeader);
                response = new JoinToGameResponseMessage(agentId,teamId,true);
            }
            catch(GameSetupException e)
            {
                response = new JoinToGameResponseMessage(agentId, teamId, false);
            }
            connection.Send(response);
            if(state.PlayerStates.Count == rules.TeamSize * 2)
            {
                start = DateTime.UtcNow;
                state.InitializePlayerPositions(rules.BoardWidth, rules.BoardHeight, rules.TeamSize);
                var rulesDict = state.GetAgentGameRules();
                foreach(var pair in state.PlayerStates)
                {
                    Message startGameMessage = new GameStartMessage(pair.Key, (int)(start - new DateTime()).TotalMilliseconds, rulesDict[pair.Key]);
                    connection.SendAsync(startGameMessage);
                }
                StartGame();
            }
        }

        public async Task GeneratePieces()
        {
            while (!state.GameEnded)
            {
                state.GeneratePiece();
                await Task.Delay(rules.PieceSpawnInterval).ConfigureAwait(false);
            }
        }

        public async Task StartGame()
        {
            var generatePiecesTask = GeneratePieces();
            //await generatePiecesTask;
        }

        private (int timestamp, int waitUntil) calcDelay(int agentId)
        {
            PlayerState agentPlayerState = state.PlayerStates[agentId];
            DateTime timestamp = DateTime.UtcNow;
            DateTime waitUntil = timestamp.AddMilliseconds(agentPlayerState.LastActionDelay);
            return ((int)(timestamp - start).TotalMilliseconds, (int)(waitUntil - start).TotalMilliseconds);
        }

        private int closestPiece(int agentId)
        {
            PlayerState agentPlayerState = state.PlayerStates[agentId];
            return state.Board[agentPlayerState.Position.X, agentPlayerState.Position.Y].Distance;
        }

        private int currectTimestamp()
        {
            return (int)(DateTime.UtcNow - start).TotalMilliseconds;
        }
        public void MoveAgent(int agentId, MoveDirection moveDirection)
        {
            Message response;
            try
            {
                state.Move(agentId, moveDirection);
                int distance = closestPiece(agentId);
                (int timestamp, int waitUntil) = calcDelay(agentId);
                response = new ActionMakeMoveResponse(agentId, timestamp, waitUntil, distance);
            }
            catch(DelayException e)
            {
                (int timestamp, int waitUntil) = calcDelay(agentId);
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch(InvalidMoveException e)
            {
                response = new InvalidMoveDirectionError(agentId, currectTimestamp());
            }
            connection.Send(response);
        }

        public void PickPiece(int agentId)
        {
            Message response;
            try
            {
                state.PickUpPiece(agentId);
                (int timestamp, int waitUntil) = calcDelay(agentId);
                response = new ActionPickPieceResponse(agentId, timestamp, waitUntil);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = calcDelay(agentId);
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch (PieceOperationException e)
            {
                response = new InvalidAction(agentId, currectTimestamp());
            }
            connection.Send(response);
        }

        public void PutPiece(int agentId)
        {
            Message response;
            try
            {
                PutPieceResult result = state.PutPiece(agentId);
                (int timestamp, int waitUntil) = calcDelay(agentId);
                response = new ActionPutPieceResponse(agentId, timestamp, waitUntil, result);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = calcDelay(agentId);
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch (PieceOperationException e)
            {
                response = new InvalidAction(agentId, currectTimestamp());
            }
            connection.Send(response);
            //TODO: Check for game end
        }

        public void Discover(int agentId)
        {
            Message response;
            try
            {
                DiscoveryResult discoveryResult = state.Discover(agentId);
                (int timestamp, int waitUntil) = calcDelay(agentId);
                response = new ActionDiscoverResponse(agentId, timestamp, waitUntil, discoveryResult);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = calcDelay(agentId);
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
                (int timestamp, int waitUntil) = calcDelay(agentId);
                response = new ActionCheckPieceResponse(agentId, timestamp, waitUntil, result);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = calcDelay(agentId);
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch (PieceOperationException e)
            {
                response = new InvalidAction(agentId, currectTimestamp());
            }
            connection.Send(response);
        }

        public void DestroyPiece(int agentId)
        {
            Message response;
            try
            {
                state.DestroyPiece(agentId);
                (int timestamp, int waitUntil) = calcDelay(agentId);
                response = new ActionDestroyPieceResponse(agentId, timestamp, waitUntil);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = calcDelay(agentId);
                response = new RequestTimePenaltyError(agentId, timestamp, waitUntil);
            }
            catch (PieceOperationException e)
            {
                response = new InvalidAction(agentId, currectTimestamp());
            }
            connection.Send(response);
        }

        public void CommunicationRequestWithData(int requesterAgentId, int targetAgentId, Object data)
        {
            try
            {
                state.SaveCommunicationData(requesterAgentId, targetAgentId, data);
                Message request = new ActionCommunicationRequest(requesterAgentId, targetAgentId, currectTimestamp());
                connection.Send(request);
            }
            catch (DelayException e)
            {
                (int timestamp, int waitUntil) = calcDelay(requesterAgentId);
                Message response = new RequestTimePenaltyError(requesterAgentId, timestamp, waitUntil);
                connection.Send(response);
            }
        }

        public void CommunicationAgreementWithData(int requesterAgentId, int targetAgentId, bool agreement, Object data)
        {
            if(agreement == false)
            {
                int timestamp = currectTimestamp();
                Message response = new ActionCommunicationResponseWithData(requesterAgentId, timestamp, timestamp, targetAgentId, false, null);
                connection.Send(response);
            }
            else
            {
                Message responseToSender, responseToTarget;
                state.DelayCommunicationPartners(requesterAgentId, targetAgentId);
                (int timestamp1, int waitUntil1) = calcDelay(requesterAgentId);
                (int timestamp2, int waitUntil2) = calcDelay(targetAgentId);
                responseToSender = new ActionCommunicationResponseWithData(requesterAgentId, timestamp1, waitUntil1, targetAgentId, true, data);
                responseToTarget = new ActionCommunicationResponseWithData(targetAgentId, timestamp2, waitUntil2, requesterAgentId, true, state.GetCommunicationData(requesterAgentId,targetAgentId));
                connection.Send(responseToSender);
                connection.Send(responseToTarget);
            }
        }
    }
}