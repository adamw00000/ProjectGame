using ConnectionLib;
using System;
using System.Collections.Generic;
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

        private Dictionary<string, Action> awaitedMessages = new Dictionary<string, Action>();
        private string lastMessageId;
        private bool waitForResponse;

        private IAgentMessageFactory unwrappedMessageFactory;
        private AgentMessageFactoryWrapper messageFactory;

        public bool? IsWinning { get; private set; } = null;

        public Agent(int tempId, DecisionModuleBase decisionModule, IConnection connection, IAgentMessageFactory agentFactory)
        {
            this.tempId = tempId;
            this.decisionModule = decisionModule;
            this.state = new AgentState();
            this.connection = connection;

            unwrappedMessageFactory = agentFactory;
            logger.Info($"Agent with temporary id {tempId} created.");
        }

        private void JoinGame(Team choosenTeam, bool wantsToBeLeader)
        {
            state.JoinGame(choosenTeam, wantsToBeLeader);
            
            Message joinMessage = unwrappedMessageFactory.CreateJoinGameMessage(choosenTeam, wantsToBeLeader);

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
                messageFactory = new AgentMessageFactoryWrapper(id, unwrappedMessageFactory);

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
                Action action = await decisionModule.ChooseAction(id, state);
                Thread.Sleep(Math.Max(1, state.WaitUntilTime - state.CurrentTimestamp()));
                action.Execute(this);
                logger.Debug($"Agent {id} sent action request: {action}");

                if (!waitForResponse)
                    continue;

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

        public void HandleStartGameMessage(int agentId, AgentGameRules rules, long absoluteStart)
        {
            logger.Debug($"Agent with temporary id {tempId} received StartGameMessage, he received id {agentId}");

            this.rules = rules;
            this.id = agentId;

            state.HandleStartGameMessage(agentId, rules, absoluteStart);

            logger.Debug($"Agent {id} - rules for the game are:\n{rules.ToString()}");
        }

        public void HandlePickPieceResponse(int timestamp, int waitUntilTime, string messageId)
        {
            if (awaitedMessages.ContainsKey(messageId))
            {
                awaitedMessages.Remove(messageId);
                logger.Debug($"Agent {id} picked up piece");
                state.PickUpPiece();
                state.WaitUntilTime = waitUntilTime;

                logger.Debug($"Agent {id} - waiting for message {lastMessageId}, received {messageId}");
                if (lastMessageId == messageId)
                    waitForResponse = false;
            }
            else
            {
                logger.Error($"Agent {id} - unexpected response received, response id: {messageId}");
                throw new InvalidOperationException("Wrong action result received");
            }
        }

        public void HandlePutPieceResponse(int timestamp, int waitUntilTime, PutPieceResult putPieceResult, string messageId)
        {
            if (awaitedMessages.ContainsKey(messageId))
            {
                awaitedMessages.Remove(messageId);
                logger.Debug($"Agent {id} put piece on the board, result: {putPieceResult.ToString()}");
                state.PlacePiece(putPieceResult);
                state.WaitUntilTime = waitUntilTime;

                logger.Debug($"Agent {id} - waiting for message {lastMessageId}, received {messageId}");
                if (lastMessageId == messageId)
                    waitForResponse = false;
            }
            else
            {
                logger.Error($"Agent {id} - unexpected response received, response id: {messageId}");
                throw new InvalidOperationException("Wrong action result received");
            }
        }

        public void HandleDestroyPieceResponse(int timestamp, int waitUntilTime, string messageId)
        {
            if (awaitedMessages.ContainsKey(messageId))
            {
                awaitedMessages.Remove(messageId);
                logger.Debug($"Agent {id} destroyed his piece");
                state.HoldsPiece = false;
                state.WaitUntilTime = waitUntilTime;

                logger.Debug($"Agent {id} - waiting for message {lastMessageId}, received {messageId}");
                if (lastMessageId == messageId)
                    waitForResponse = false;
            }
            else
            {
                logger.Error($"Agent {id} - unexpected response received, response id: {messageId}");
                throw new InvalidOperationException("Wrong action result received");
            }
        }

        public void HandleMoveResponse(int timestamp, int waitUntilTime, int distance, string messageId)
        {
            if (awaitedMessages.ContainsKey(messageId))
            {
                MoveDirection direction = ((ActionMove)awaitedMessages[messageId]).Direction;
                awaitedMessages.Remove(messageId);
                logger.Debug($"Agent {id} - current time: {state.CurrentTimestamp()}, wait until: {waitUntilTime}");
                state.Move(direction, distance); //Needs to be fixed with collection of actions
                state.WaitUntilTime = waitUntilTime;

                logger.Debug($"Agent {id} - waiting for message {lastMessageId}, received {messageId}");
                if (lastMessageId == messageId)
                    waitForResponse = false;

                logger.Debug($"Agent {id} moved, his new position: {state.Position}, distance to closest Piece: {distance}");
            }
            else
            {
                logger.Error($"Agent {id} - unexpected response received, response id: {messageId}");
                throw new InvalidOperationException("Wrong action result received");
            }
        }

        public void HandleDiscoverResponse(int timestamp, int waitUntilTime, DiscoveryResult closestPieces, string messageId)
        {
            if (awaitedMessages.ContainsKey(messageId))
            {
                awaitedMessages.Remove(messageId);
                logger.Debug($"Agent {id} discovered his surroundings");
                state.Discover(closestPieces, timestamp);
                state.WaitUntilTime = waitUntilTime;

                logger.Debug($"Agent {id} - waiting for message {lastMessageId}, received {messageId}");
                if (lastMessageId == messageId)
                    waitForResponse = false;
            }
            else
            {
                logger.Error($"Agent {id} - unexpected response received, response id: {messageId}");
                throw new InvalidOperationException("Wrong action result received");
            }
        }

        public void HandleCheckPieceResponse(int timestamp, int waitUntilTime, bool isValid, string messageId)
        {
            if (awaitedMessages.ContainsKey(messageId))
            {
                awaitedMessages.Remove(messageId);
                logger.Debug($"Agent {id} checked his piece validity - it is {(isValid ? "valid" : "invalid")}");
                state.WaitUntilTime = waitUntilTime;

                logger.Debug($"Agent {id} - waiting for message {lastMessageId}, received {messageId}");
                if (lastMessageId == messageId)
                    waitForResponse = false;

                state.PieceState = isValid ? PieceState.Valid : PieceState.Invalid;
            }
            else
            {
                logger.Error($"Agent {id} - unexpected response received, response id: {messageId}");
                throw new InvalidOperationException("Wrong action result received");
            }
        }

        public void HandleCommunicationRequest(int requesterId, int timestamp)
        {
            logger.Debug($"Agent {id} received CommunicationRequest from agent {requesterId}");

            decisionModule.AddSenderToCommunicationQueue(state, requesterId);
        }

        public void HandleCommunicationResponse(int timestamp, int waitUntilTime, int senderId, bool agreement, object data, string messageId)
        {
            try
            {
                if (awaitedMessages.ContainsKey(messageId))
                {
                    awaitedMessages.Remove(messageId);
                    logger.Debug($"Agent {id} received communication response from agent {senderId}, he " + (agreement ? "agreed" : "didn't agree") + " for the communication");
                    decisionModule.SaveCommunicationResult(senderId, agreement, state.Start.AddMilliseconds(timestamp), data, state);

                    state.WaitUntilTime = waitUntilTime;

                    logger.Debug($"Agent {id} - waiting for message {lastMessageId}, received {messageId}");
                    if (lastMessageId == messageId)
                        waitForResponse = false;
                }
                else
                {
                    logger.Error($"Agent {id} - unexpected response received, response id: {messageId}");
                    throw new InvalidOperationException("Wrong action result received");
                }
            }
            catch (InvalidCommunicationDataException e)
            {
                logger.Error(e);
            }
        }

        public void HandleTimePenaltyError(int timestamp, int waitUntilTime, string messageId)
        {
            logger.Warn($"Agent {id} tried to move during penalty.");
            
            state.WaitUntilTime = waitUntilTime;
            waitForResponse = false;
        }

        public void HandleInvalidMoveDirectionError(int timestamp, string messageId)
        {
            logger.Warn($"Agent {id} tried to make invalid move.");
            waitForResponse = false;
        }

        public void HandleInvalidActionError(int timestamp, string messageId)
        {
            logger.Warn($"Agent {id} tried to perform invalid action.");
            waitForResponse = false;
        }

        public void EndGame(Team winningTeam, int timestamp)
        {
            IsWinning = winningTeam == state.Team;
            logger.Debug($"Agent {id} finished the game, he {(IsWinning.Value ? "won" : "lost")}.");
         
            state.GameEnded = true;
            waitForResponse = false;
        }

        public void Move(ActionMove action)
        {
            waitForResponse = true;
            (Message message, string messageId) = messageFactory.MoveMessage(action.Direction);
            SendAndAddToCollection(message, messageId, action);
        }

        public void CheckPiece(ActionCheckPiece action)
        {
            waitForResponse = true;
            (Message message, string messageId) = messageFactory.CheckPieceMessage();
            SendAndAddToCollection(message, messageId, action);
        }

        public void DestroyPiece(ActionDestroyPiece action)
        {
            waitForResponse = true;
            (Message message, string messageId) = messageFactory.DestroyMessage();
            SendAndAddToCollection(message, messageId, action);
        }

        public void PutPiece(ActionPutPiece action)
        {
            waitForResponse = true;
            (Message message, string messageId) = messageFactory.PutPieceMessage();
            SendAndAddToCollection(message, messageId, action);
        }

        public void PickPiece(ActionPickPiece action)
        {
            waitForResponse = true;
            (Message message, string messageId) = messageFactory.PickPieceMessage();
            SendAndAddToCollection(message, messageId, action);
        }

        public void Communicate(ActionCommunicate action)
        {
            (Message message, string messageId) = messageFactory.CommunicationRequestMessage(action.TargetId, action.Data);
            SendAndAddToCollection(message, messageId, action);
        }

        public void Discover(ActionDiscovery action)
        {
            waitForResponse = true;
            (Message message, string messageId) = messageFactory.DiscoveryMessage();
            SendAndAddToCollection(message, messageId, action);
        }

        public void AgreeOnCommunication(ActionCommunicationAgreement action)
        {
            waitForResponse = action.Agreement;
            (Message message, string messageId) = messageFactory.CommunicationAgreementMessage(action.RequesterId, action.Agreement, action.Data);
            SendAndAddToCollection(message, messageId, action);
        }

        private void SendAndAddToCollection(Message message, string messageId, Action action)
        {
            awaitedMessages.Add(messageId, action);
            lastMessageId = messageId;
            logger.Debug($"Agent {id} sent message {messageId}");
            connection.Send(message);
        }
    }
}