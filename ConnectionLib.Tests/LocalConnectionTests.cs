using System;
using Xunit;

using ConnectionLib;

namespace ConnectionLib.Tests
{
    class TestMessage: Message
    {
        public string Text { get; set; }

        public TestMessage(int agentId, string text): base(agentId)
        {
            Text = text;
        }
    }

    // W niektórych miejscach jest AgentId = int.MaxValue, chcê przez to pokazaæ ¿e wartoœæ wpisana w tamtym miejscu nie jest wa¿na.
    // (Na etapie testów).
    public class LocalConnectionTests
    {
        [Fact]
        private void LocalConnection_WhenCreated_HasConnectedState()
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            AgentLocalConnection agentLocalConnection = new AgentLocalConnection(communicationServer);
            GMLocalConnection gmLocalConnection = new GMLocalConnection(communicationServer);

            Assert.True(agentLocalConnection.Connected);
            Assert.True(gmLocalConnection.Connected);
        }

        [Fact]
        private void LocalConnection_WhenDisconnected_HasDisconnectedState()
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            AgentLocalConnection agentLocalConnection = new AgentLocalConnection(communicationServer);
            GMLocalConnection gmLocalConnection = new GMLocalConnection(communicationServer);

            agentLocalConnection.Disconnect();
            gmLocalConnection.Disconnect();

            Assert.False(agentLocalConnection.Connected);
            Assert.False(gmLocalConnection.Connected);
        }

        [Fact]
        private void LocalConnection_WhenDisconnected_ThrowExceptionOnSend()
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            AgentLocalConnection agentLocalConnection = new AgentLocalConnection(communicationServer);
            GMLocalConnection gmLocalConnection = new GMLocalConnection(communicationServer);

            agentLocalConnection.Disconnect();
            gmLocalConnection.Disconnect();

            Assert.Throws<Exception>(() => agentLocalConnection.Send(null));
            Assert.Throws<Exception>(() => gmLocalConnection.Send(null));

            Assert.ThrowsAsync<Exception>(() => agentLocalConnection.SendAsync(null));
            Assert.ThrowsAsync<Exception>(() => gmLocalConnection.SendAsync(null));
        }

        [Fact]
        private void LocalConnection_WhenDisconnected_ThrowExceptionOnReceive()
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            AgentLocalConnection agentLocalConnection = new AgentLocalConnection(communicationServer);
            GMLocalConnection gmLocalConnection = new GMLocalConnection(communicationServer);

            agentLocalConnection.Disconnect();
            gmLocalConnection.Disconnect();

            Assert.Throws<Exception>(() => agentLocalConnection.Receive());
            Assert.Throws<Exception>(() => gmLocalConnection.Receive());

            Assert.ThrowsAsync<Exception>(() => agentLocalConnection.ReceiveAsync());
            Assert.ThrowsAsync<Exception>(() => gmLocalConnection.ReceiveAsync());
        }

        // Te poni¿sze testy nie s¹ zbyt jednostkowe bo korzystaj¹ z LocalCommunicationServer...
        // no ale tak czy siak je napisa³em bo pokazuj¹ ¿e implementacja dzia³a
        [Fact]
        private void AgentLocalConnection_WhenMessageSent_ItIsDelivered()
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            AgentLocalConnection agentLocalConnection = new AgentLocalConnection(communicationServer);
            GMLocalConnection gmLocalConnection = new GMLocalConnection(communicationServer);

            Message message = new Message(int.MaxValue);

            agentLocalConnection.Send(message);
            Message receivedMessage = gmLocalConnection.Receive();

            Assert.Equal(message, receivedMessage);
        }

        [Fact]
        private void GMLocalConnection_WhenMessageSent_ItIsDelivered()
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            AgentLocalConnection agentLocalConnection = new AgentLocalConnection(communicationServer);
            GMLocalConnection gmLocalConnection = new GMLocalConnection(communicationServer);

            // to jest slaby test, bo ja z powodu determinizmu communicationServer wiem ¿e agent dostanie id 1 ale bêdzie problem jak siê np implementacja zmieni.
            Message message = new Message(1);

            gmLocalConnection.Send(message);
            Message receivedMessage = agentLocalConnection.Receive();

            Assert.Equal(message, receivedMessage);
        }

        [Fact]
        private void LocalCommunicationServer_WhenSeveralMessagesSent_TheyAreDeliverdInRightOrder()
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            AgentLocalConnection agentLocalConnection = new AgentLocalConnection(communicationServer);
            GMLocalConnection gmLocalConnection = new GMLocalConnection(communicationServer);

            Message message1 = new Message(int.MaxValue);
            Message message2 = new Message(int.MaxValue);

            agentLocalConnection.Send(message1);
            agentLocalConnection.Send(message2);

            Message receivedMessage1 = gmLocalConnection.Receive();
            Message receivedMessage2 = gmLocalConnection.Receive();

            Assert.Equal(message1, receivedMessage1);
            Assert.Equal(message2, receivedMessage2);
        }

        [Fact]
        private void LocalCommunicationServer_WhenSeveralAgentsConnected_MessagesHaveDifferentAgentId()
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            GMLocalConnection gm = new GMLocalConnection(communicationServer);
            AgentLocalConnection agent1 = new AgentLocalConnection(communicationServer);
            AgentLocalConnection agent2 = new AgentLocalConnection(communicationServer);

            Message message1 = new Message(int.MaxValue);
            Message message2 = new Message(int.MaxValue);

            agent1.Send(message1);
            agent2.Send(message2);

            Assert.NotEqual(message1.AgentId, message2.AgentId);
        }

        [Fact]
        private void LocalCommunicationServer_WhenSeveralAgentsConnected_CorrectAgentsReceiveMessages()
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            GMLocalConnection gm = new GMLocalConnection(communicationServer);
            AgentLocalConnection agent1 = new AgentLocalConnection(communicationServer);
            AgentLocalConnection agent2 = new AgentLocalConnection(communicationServer);

            Message message1 = new Message(int.MaxValue);
            Message message2 = new Message(int.MaxValue);

            agent1.Send(message1);
            agent2.Send(message2);

            Message gmReceived1 = gm.Receive();
            Message gmReceived2 = gm.Receive();

            gm.Send(gmReceived1);
            gm.Send(gmReceived2);

            Message agentReceived1 = agent1.Receive();
            Message agentReceived2 = agent2.Receive();

            Assert.Equal(message1, agentReceived1);
            Assert.Equal(message2, agentReceived2);
        }
    }
}
