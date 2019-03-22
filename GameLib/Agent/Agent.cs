using ConnectionLib;
using System;
using System.Threading.Tasks;

namespace GameLib
{
    public class Agent
    {
        private readonly IConnection connection;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly int id;
        private readonly IDecisionModule decisionModule;
        private readonly AgentState state;
        private readonly GameRules rules;

        public Agent(int id, IDecisionModule decisionModule) // id - temporary (to be changed when Agent receives JoinGameResponseMessage)
        {
            this.id = id;
            this.decisionModule = decisionModule;
            this.state = new AgentState();
        }

        public void JoinGame(string serverAddress, int port)
        {
            throw new NotImplementedException();
        }

        public async Task Run()
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
                await decisionModule.ChooseAction(id, state);
                await Task.Delay(500);
            }


            throw new NotImplementedException();
        }

        public void ServeCommunicationRequest(int requesterId)
        {
            throw new NotImplementedException();
        }

        public void HandleCommunicationResponse(int timestamp, int waitUntilTime, int senderId, bool agreement, object data)
        {
            //timestamp => datetime
            try
            {
                decisionModule.SaveCommunicationResult(senderId, agreement, /*temporary*/ new DateTime(), data, state);
            }
            catch (InvalidCommunicationDataException e)
            {
                logger.Error(e);
            }
        }
    }
}