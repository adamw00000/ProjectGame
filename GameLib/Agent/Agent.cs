using ConnectionLib;
using GameLib.Actions;
using GameLib.GameMessages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameLib
{
    public class Agent
    {
        private readonly IConnection connection;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private int id;
        private readonly IDecisionModule decisionModule;
        private readonly AgentState state;
        private GameRules rules;
        private DateTime start;
        private bool waitForResponse;
        private bool gameStarted = false;
        private bool gameEnded = false;
        private Message awaitedForResponse;
        private int isWinning = -1;
        private bool wantsToBeLeader;
        private bool isLeader;
        private int[] teamIds;
        private Team team;
        private bool isInGame = false;

        private int CurrentTimestamp()
        {
            return (int)(DateTime.UtcNow - start).TotalMilliseconds;
        }
        public void HandlePickPieceResponse(int timestamp, int waitUntilTime)
        {
            if (awaitedForResponse is ActionPickPiece)
            {
                state.PickUpPiece();
                state.WaitUntilTime = waitUntilTime;
                waitForResponse = false;
            }
            else
                throw new InvalidOperationException("Wrong action result received");
        }

        public void HandlePutPieceResponse(int timestamp, int waitUntilTime, PutPieceResult putPieceResult)
        {
            if (awaitedForResponse is ActionPutPiece)
            {
                state.PlacePiece(putPieceResult);
                state.WaitUntilTime = waitUntilTime;
                waitForResponse = false;
            }
            else
                throw new InvalidOperationException("Wrong action result received");
        }

        public void HandleDestroyPieceResponse(int timestamp, int waitUntilTime)
        {
            if (awaitedForResponse is ActionDestroyPiece)
            {
                state.HoldsPiece = false;
                state.WaitUntilTime = waitUntilTime;
                waitForResponse = false;
            }
            else
                throw new InvalidOperationException("Wrong action result received");
        }

        public void HandleMoveResponse(int timestamp, int waitUntilTime, int distance)
        {
            if (awaitedForResponse is ActionMove move)
            {
                state.Move(move.MoveDirection, distance);
                state.WaitUntilTime = waitUntilTime;
                waitForResponse = false;
            }
            else
                throw new InvalidOperationException("Wrong action result received");
        }

        public void HandleDiscoverResponse(int timestamp, int waitUntilTime, DiscoveryResult closestPieces)
        {
            if (awaitedForResponse is ActionDiscovery)
            {
                state.Discover(closestPieces, timestamp);
                state.WaitUntilTime = waitUntilTime;
                waitForResponse = false;
            }
            else
                throw new InvalidOperationException("Wrong action result received");
        }

        public void HandleJoinResponse(bool isConnected)
        {
            isInGame = isConnected;
        }

        public void HandleCheckPieceResponse(int timestamp, int waitUntilTime, bool isValid)
        {
            if (awaitedForResponse is ActionCheckPiece)
            {
                state.WaitUntilTime = waitUntilTime;
                waitForResponse = false;
                state.PieceState = isValid ? PieceState.Valid : PieceState.Invalid;
            }
            else
                throw new InvalidOperationException("Wrong action result received");
        }
        public void HandleCommunicationRequest(int requesterId, int timestamp)
        {
            Message response = new ActionCommunicationAgreementWithData(requesterId, id, false, null);
            connection.Send(response);
            //Needs to be developed
        }

        public void HandleCommunicationResponse(int timestamp, int waitUntilTime, int senderId, bool agreement, object data)
        {
            if (agreement)
            {
                throw new NotImplementedException();
            }
            //TODO: Response handling
            //This can't throw exception, because it's called in case of rejected communication.
        }

        public void HandleStartGameMessage(int agentId, GameRules rules, int timestamp)
        {
            this.id = agentId;
            this.isLeader = this.id == rules.TeamLiderId;
            this.teamIds = (int[])rules.AgentIdsFromTeam.Clone();
            this.rules = rules;
            state.Setup(rules);
            start = DateTime.UtcNow;
            gameStarted = true;
        }

        public void EndGame(int winningTeam, int timestamp)
        {
            gameEnded = true;
        }

        public Agent(int id, IDecisionModule decisionModule, IConnection connection)
        {
            this.id = id;
            this.decisionModule = decisionModule;
            this.state = new AgentState();
            this.connection = connection;
        }

        private void JoinGame(Team choosenTeam, bool wantsToBeLeader)
        {
            this.team = choosenTeam;
            this.wantsToBeLeader = wantsToBeLeader;
            Message joinMessage = new JoinGameMessage(id, (int)choosenTeam, wantsToBeLeader);
            connection.Send(joinMessage);
            while (!gameStarted)
            {
                Message message = connection.Receive();
                message.Handle(this);
            }
        }

        public async Task Run(Team choosenTeam, bool wantsToBeLeader = false)
        {
            JoinGame(choosenTeam, wantsToBeLeader);
            if(isInGame)
            {
                try
                {
                    await MainLoopAsync();
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }

        }

        public void HandleTimePenaltyError(int timestamp, int waitUntilTime)
        {
            state.WaitUntilTime = waitUntilTime;
            waitForResponse = false;
        }

        public void HandleInvalidMoveDirectionError(int timestamp)
        {
            waitForResponse = false;
        }

        public void HandleInvalidActionError(int timestamp)
        {
            waitForResponse = false;
        }

        private async Task MainLoopAsync()
        {
            while (!gameEnded)
            {
                Message action = (Message) (await decisionModule.ChooseAction(id, state));
                Thread.Sleep(Math.Max(1, state.WaitUntilTime - CurrentTimestamp()));
                connection.Send(action);
                if (action is ActionCommunicationRequestWithData)
                    continue;

                if (action is ActionCommunicationAgreementWithData response && !response.AcceptsCommunication)
                    continue;

                waitForResponse = true;
                awaitedForResponse = action;

                do
                {
                    Message msg = connection.Receive();
                    msg.Handle(this);
                } while (waitForResponse);

                while (CurrentTimestamp() < state.WaitUntilTime)
                {

                    bool res = connection.TryReceive(out Message m, state.WaitUntilTime - CurrentTimestamp());
                    if (res)
                    {
                        m.Handle(this);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}