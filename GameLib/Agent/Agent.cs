using ConnectionLib;
using System;

namespace GameLib
{
    public class Agent
    {
        private readonly IConnection connection;

        private readonly int id;
        private readonly IDecisionModule decisionModule;
        private readonly AgentState state;
        private readonly GameRules rules;

        public Agent(IDecisionModule decisionModule)
        {
            this.decisionModule = decisionModule;
            this.state = new AgentState();
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
            while (true)
            {
                decisionModule.ChooseAction(id, state);
                System.Threading.Thread.Sleep(500);
            }


            throw new NotImplementedException();
        }

        public void ServeCommunicationRequest(int requesterId)
        {
            throw new NotImplementedException();
        }
    }
}