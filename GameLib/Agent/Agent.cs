using ConnectionLib;
using GameLib.Actions;
using GameLib.GameMessages;
using System;
using System.Threading.Tasks;

namespace GameLib
{
    public class Agent
    {
        private readonly IConnection connection;

        private int id;
        private readonly IDecisionModule decisionModule;
        private readonly AgentState state;
        private GameRules rules;
        private Task loop;
        private bool waitForResponse;
        private Message awaitedForResponse;
        private int isWinning = -1;
        private bool wantsToBeLeader;
        private bool isLeader;
        private bool gameStarted = false;
        private int[] teamIds;
        private Team team;

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
            if (!isConnected)
                throw new InvalidOperationException();
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

        public void HandleCommunicationResponse(int timestamp, int waitUntilTime, int senderId, bool agreement, object data)
        {
            if (agreement)
            {
                throw new NotImplementedException();
            }
            //TODO: Response handling
            //This can't throw exception, because it's called in case of rejected communication.
        }

        public void StartGame(int agentId, GameRules rules, int timestamp)
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

        private bool gameEnded;
        private DateTime start;
        public Agent(IDecisionModule decisionModule, IConnection connection)
        {
            this.decisionModule = decisionModule;
            this.state = new AgentState();
            this.connection = connection;
        }

        public async void JoinGame(Team choosenTeam, bool wantsToBeLeader = false)
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
            Run();
        }

        public void Run()
        {
            if (loop != null)
            {
                //Loop already running
                throw new InvalidOperationException("Agent is already running");
            }
            mainLoop();
        }

        public void HandleCommunicationRequest(int requesterId, int timestamp)
        {
            Message response = new ActionCommunicationAgreementWithData(requesterId, id, false, null);
            connection.Send(response);
            //TODO: Develop way to handle this kind of request
        }

        public void HandleTimePenaltyError(int timestamp, int waitUntilTime)
        {
            this.state.WaitUntilTime = waitUntilTime;
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

        private void mainLoop()
        {
            while (!gameEnded)
            {
                Message action = (Message)decisionModule.ChooseAction(id, state);
                connection.Send(action);

                if (action is ActionCommunicationRequestWithData)
                    continue;

                waitForResponse = true;
                awaitedForResponse = action;

                do
                {
                    Message msg = connection.Receive();
                    msg.Handle(this);
                } while (waitForResponse);

                while ((DateTime.UtcNow - start).TotalMilliseconds < state.WaitUntilTime)
                {
                    bool res = connection.TryReceive(out Message m, (int)(DateTime.UtcNow - start).TotalMilliseconds);
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