using ConnectionLib;
using GameLib.Actions;
using GameLib.GameMessages;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameLib
{
    public class Agent
    {
        private readonly IConnection connection;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private int id;
        private readonly int tempId;
        private readonly DecisionModuleBase decisionModule;
        private readonly AgentState state;
        private AgentGameRules rules;

        private Message awaitedForResponse;
        private bool waitForResponse;
        private int LastActionTimestamp;

        // Not used - did u miss it somewhere?
        //private int isWinning = -1;

        public Agent(int tempId, DecisionModuleBase decisionModule, IConnection connection)
        {
            this.tempId = tempId;
            this.decisionModule = decisionModule;
            this.state = new AgentState();
            this.connection = connection;
            logger.Info($"Agent with temporary id {tempId} created.");
        }

        private void JoinGame(Team choosenTeam, bool wantsToBeLeader)
        {
            state.JoinGame(choosenTeam, wantsToBeLeader);
            
            Message joinMessage = new JoinGameMessage(tempId, (int)choosenTeam, wantsToBeLeader); //used to be "JoinGameMessage(id,... )". By mistake?
            connection.Send(joinMessage);
            logger.Debug($"Agent with temporary id {tempId} sent JoinGameMessage. He wants to join team {(choosenTeam == Team.Blue ? "Blue" : "Red")} and {(wantsToBeLeader ? "wants" : "doesn't want")} to be a leader.");

            while (!state.GameStarted)
            {
                Message message = connection.Receive();
                message.Handle(this);
            }
            logger.Info($"Agent with temporary id {tempId}: the game has started. {(state.IsInGame ? "He joined the game successfully, received id " + id + " and he " + (state.IsLeader ? "is" : "is not") + " a leader" : "He failed to join the game")}.");
        }

        public async Task Run(Team choosenTeam, bool wantsToBeLeader = false)
        {
            JoinGame(choosenTeam, wantsToBeLeader);
            if(state.IsInGame)
            {
                try
                {
                    logger.Info($"Agent {id} entered the game successfully, starting the main loop.");
                    await MainLoopAsync();
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }

        }

        private async Task MainLoopAsync()
        {
            while (!state.GameEnded)
            {
                AgentMessage actionTemp = (AgentMessage)await decisionModule.ChooseAction(id, state);
                Message action = (Message)actionTemp;
                Thread.Sleep(Math.Max(1, state.WaitUntilTime - state.CurrentTimestamp()));
                logger.Debug($"Agent {id} sent action request: {action}");
                connection.Send(action);


                if (action is ActionCommunicationRequestWithData)
                    continue;

                if (action is ActionCommunicationAgreementWithData response && !response.AcceptsCommunication)
                    continue;

                LastActionTimestamp = actionTemp.Timestamp;
                waitForResponse = true;
                awaitedForResponse = action;

                do
                {
                    Message msg = connection.Receive();
                    msg.Handle(this);
                    logger.Trace($"Agent {id} handled message while waiting for response");
                } while (waitForResponse);

                logger.Debug($"Agent {id} stopped waiting for response, last action delay left: {(state.WaitUntilTime - state.CurrentTimestamp() <= 0 ? "no delay" : (state.WaitUntilTime - state.CurrentTimestamp()) + "ms")}");

                while (state.CurrentTimestamp() < state.WaitUntilTime)
                {
                    bool res = connection.TryReceive(out Message m, state.WaitUntilTime - state.CurrentTimestamp());
                    if (res)
                    {
                        logger.Debug($"Agent {id} handled message while delayed");
                        m.Handle(this);
                    }
                    else
                    {
                        logger.Debug($"Agent {id} stopped waiting");
                        break;
                    }
                }
            }
        }

        public void HandleJoinResponse(bool isConnected)
        {
            if (!isConnected)
                logger.Warn($"Agent with temporary id {tempId} didn't connect to the game");

            state.IsInGame = isConnected;
        }

        public void HandleStartGameMessage(int agentId, AgentGameRules rules, int timestamp, long absoluteStart)
        {
            logger.Debug($"Agent with temporary id {tempId} received StartGameMessage, he received id {agentId}");

            this.rules = rules;
            this.id = agentId;

            state.HandleStartGameMessage(agentId, rules, timestamp, absoluteStart);

            logger.Debug($"Agent {id} - rules for the game are:\n{rules.ToString()}");
        }

        public void HandlePickPieceResponse(int timestamp, int waitUntilTime)
        {
            if (awaitedForResponse is ActionPickPiece)
            {
                logger.Debug($"Agent {id} picked up piece");
                state.PickUpPiece();
                state.WaitUntilTime = waitUntilTime;
                waitForResponse = false;
            }
            else
            {
                logger.Error($"Agent {id} - wrong action response received - PickPiece response expected");
                //throw new InvalidOperationException("Wrong action result received");
            }
        }

        public void HandlePutPieceResponse(int timestamp, int waitUntilTime, PutPieceResult putPieceResult)
        {
            if (awaitedForResponse is ActionPutPiece)
            {
                logger.Debug($"Agent {id} put piece on the board, result: {putPieceResult.ToString()}");
                state.PlacePiece(putPieceResult);
                state.WaitUntilTime = waitUntilTime;
                waitForResponse = false;
            }
            else
            {
                logger.Error($"Agent {id} - wrong action response received - PutPiece response expected");
                //throw new InvalidOperationException("Wrong action result received");
            }
        }

        public void HandleDestroyPieceResponse(int timestamp, int waitUntilTime)
        {
            if (awaitedForResponse is ActionDestroyPiece)
            {
                logger.Debug($"Agent {id} destroyed his piece");
                state.HoldsPiece = false;
                state.WaitUntilTime = waitUntilTime;
                waitForResponse = false;
            }
            else
            {
                logger.Error($"Agent {id} - wrong action response received - Destroy response expected");
                //throw new InvalidOperationException("Wrong action result received");
            }
        }

        public void HandleMoveResponse(int timestamp, int waitUntilTime, int distance)
        {
            if (awaitedForResponse is ActionMove move)
            {
                logger.Debug($"Agent {id} - current time: {state.CurrentTimestamp()}, wait until: {waitUntilTime}");
                state.Move(move.MoveDirection, distance);
                state.WaitUntilTime = waitUntilTime;
                waitForResponse = false;

                logger.Debug($"Agent {id} moved, his new position: {state.Position}, distance to closest Piece: {distance}");
            }
            else
            {
                logger.Error($"Agent {id} - wrong action response received - Move response expected");
                //throw new InvalidOperationException("Wrong action result received");
            }
        }

        public void HandleDiscoverResponse(int timestamp, int waitUntilTime, DiscoveryResult closestPieces)
        {
            if (awaitedForResponse is ActionDiscovery)
            {
                logger.Debug($"Agent {id} discovered his surroundings");
                state.Discover(closestPieces, timestamp);
                state.WaitUntilTime = waitUntilTime;
                waitForResponse = false;
            }
            else
            {
                logger.Error($"Agent {id} - wrong action response received - Discover response expected");
                //throw new InvalidOperationException("Wrong action result received");
            }
        }

        public void HandleCheckPieceResponse(int timestamp, int waitUntilTime, bool isValid)
        {
            if (awaitedForResponse is ActionCheckPiece)
            {
                logger.Debug($"Agent {id} checked his piece validity - it is {(isValid ? "valid" : "invalid")}");
                state.WaitUntilTime = waitUntilTime;
                waitForResponse = false;
                state.PieceState = isValid ? PieceState.Valid : PieceState.Invalid;
            }
            else
            {
                logger.Error($"Agent {id} - wrong action response received - CheckPiece response expected");
                //throw new InvalidOperationException("Wrong action result received");
            }
        }

        public void HandleCommunicationRequest(int requesterId, int timestamp)
        {
            logger.Debug($"Agent {id} received CommunicationRequest from agent {requesterId}");

            decisionModule.AddSenderToCommunicationQueue(state, requesterId);
        }

        public void HandleCommunicationResponse(int timestamp, int waitUntilTime, int senderId, bool agreement, object data)
        {
            try
            {
                logger.Debug($"Agent {id} received communication response from agent {senderId}, he " + (agreement ? "agreed" : "didn't agree") + " for the communication");
                decisionModule.SaveCommunicationResult(senderId, agreement, state.Start.AddMilliseconds(timestamp), data, state);
                
                state.WaitUntilTime = waitUntilTime;
                waitForResponse = false;
            }
            catch (InvalidCommunicationDataException e)
            {
                logger.Error(e);
            }
        }

        public void HandleTimePenaltyError(int timestamp, int requestTimestamp, int waitUntilTime)
        {
            logger.Warn($"Agent {id} tried to move during penalty.");
            
            state.WaitUntilTime = waitUntilTime;
            if (requestTimestamp == LastActionTimestamp)
            {
                waitForResponse = false;
            }
        }

        public void HandleInvalidMoveDirectionError(int timestamp, int requestTimestamp)
        {
            logger.Warn($"Agent {id} tried to make invalid move.");

            if (requestTimestamp == LastActionTimestamp)
            {
                waitForResponse = false;
            }
        }

        public void HandleInvalidActionError(int timestamp, int requestTimestamp)
        {
            logger.Warn($"Agent {id} tried to perform invalid action.");

            if (requestTimestamp == LastActionTimestamp)
            {
                waitForResponse = false;
            }
        }

        public void EndGame(int winningTeam, int timestamp)
        {
            logger.Debug($"Agent {id} finished the game, he {(winningTeam == (int)state.Team ? "won" : "lost")}.");

            state.GameEnded = true;
            waitForResponse = false;
        }
    }
}