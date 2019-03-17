using Shouldly;
using System;
using Xunit;
using ConnectionLib;
using System.Reflection;

namespace GameLib.Tests
{
    public class ActionTests
    {
        private (GameMaster, Agent, Agent) createGMandAgents(IDecisionModule decisionModule = null)
        {
            LocalCommunicationServer localCommunicationServer = new LocalCommunicationServer();
            AgentLocalConnection agentLocalConnection1 = new AgentLocalConnection(localCommunicationServer);
            AgentLocalConnection agentLocalConnection2 = new AgentLocalConnection(localCommunicationServer);
            GMLocalConnection gMLocalConnection = new GMLocalConnection(localCommunicationServer);
            Agent agent1 = new Agent(decisionModule, agentLocalConnection1);
            Agent agent2 = new Agent(decisionModule, agentLocalConnection2);
            GameMaster gameMaster = new GameMaster(null,gMLocalConnection);
            return (gameMaster, agent1, agent2);
        }
    }
}
