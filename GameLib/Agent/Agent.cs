using ConnectionLib;
using GameLib.Actions;
using System;
using System.Threading.Tasks;

namespace GameLib
{
    public class Agent
    {
        private readonly IConnection connection;

        private readonly int id;
        private readonly IDecisionModule decisionModule;
        private readonly AgentState state;
        private GameRules rules;
        private Task loop;
        private bool waitForResponse;
        private Message awaitedForResponse;

        public void HandlePickPieceResponse(int timestamp, int waitUntilTime)
        {
            if (awaitedForResponse is ActionPickPiece)
            {
                state.PickUpPiece();
                state.waitUntilTime = waitUntilTime;
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
                state.waitUntilTime = waitUntilTime;
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
                state.waitUntilTime = waitUntilTime;
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
                state.waitUntilTime = waitUntilTime;
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
                state.waitUntilTime = waitUntilTime;
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

        public void HandleCheckPieceResponse(int timestamp, int waitUntilTime, bool isTrue)
        {
            if(awaitedForResponse is ActionCheckPiece)
            {
                state.waitUntilTime = waitUntilTime;
                waitForResponse = false;
                state.PieceState = isTrue ? PieceState.Valid : PieceState.Invalid;
            }
            else
                throw new InvalidOperationException("Wrong action result received");
        }

        public void HandleCommunicationResponse(int timestamp, int waitUntilTime, int senderId, bool agreement, object data)
        {
            
        }

        public void StartGame(GameRules rules, int timestamp)
        {
            this.rules = rules;
            start = (new DateTime()).AddMilliseconds(timestamp);
            Run();
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

        public void JoinGame(string serverAddress, int port)
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            //bool gameEnded = false; //?
            //while(!gameEnded)
            //{
            //    IAction nextAction = decisionModule.ChooseAction(state);
            //    connection.Send(nextAction);//????
            //}
            // loop decisionModule <-> connection.Send()
            if(loop != null)
            {
                //Loop alraedy running
                throw new InvalidOperationException("Agent is already running");
            }
            loop = new Task(mainLoop);
        }

        public void ServeCommunicationRequest(int requesterId)
        {
            throw new NotImplementedException();
        }

        public void HandleTimePenaltyError(int timestamp, int waitUntilTime)
        {
            throw new NotImplementedException();
        }

        public void HandleInvalidMoveDirectionError(int timestamp)
        {
            throw new NotImplementedException();
        }

        public void HandleInvalidActionError(int timestamp)
        {
            throw new NotImplementedException();
        }

        private void mainLoop()
        {
            while (gameEnded)
            {
                Message action = (Message) decisionModule.ChooseAction(id, state);
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
                while((DateTime.UtcNow - start).TotalMilliseconds < state.waitUntilTime)
                {
                    bool res = connection.TryReceive(out Message m, (int)(DateTime.UtcNow - start).TotalMilliseconds);
                    if(res)
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