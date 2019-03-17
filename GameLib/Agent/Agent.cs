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
        private readonly GameRules rules;
        private Task loop;
        private bool waitForResponse;
        private Message awaitedForResponse;

        internal void HandleJoinResponse(bool isConnected)
        {
            throw new NotImplementedException();
        }

        internal void StartGame(GameRules rules, int timestamp)
        {
            throw new NotImplementedException();
        }

        internal void EndGame(int winningTeam, int timestamp)
        {
            throw new NotImplementedException();
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