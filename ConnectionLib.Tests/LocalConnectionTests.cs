using System;
using Xunit;
using Shouldly;
using ConnectionLib;

namespace ConnectionLib.Tests
{
    class TestMessage: Message
    {
        public string Text { get; set; }

        public TestMessage(int agentId, string text = ""): base(agentId)
        {
            Text = text;
        }

        public override void Handle(object handler)
        {
        }
    }

    // I am setting AgentId = int.MaxValue to showcase that it does not matter when it comes to testing.
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
        private void LocalConnection_WhenDisconnected_ThrowInvalidOperationExceptionOnSend()
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            AgentLocalConnection agentLocalConnection = new AgentLocalConnection(communicationServer);
            GMLocalConnection gmLocalConnection = new GMLocalConnection(communicationServer);

            agentLocalConnection.Disconnect();
            gmLocalConnection.Disconnect();

            Assert.Throws<InvalidOperationException>(() => agentLocalConnection.Send(null));
            Assert.Throws<InvalidOperationException>(() => gmLocalConnection.Send(null));

            Assert.ThrowsAsync<InvalidOperationException>(() => agentLocalConnection.SendAsync(null));
            Assert.ThrowsAsync<InvalidOperationException>(() => gmLocalConnection.SendAsync(null));
        }

        [Fact]
        private void LocalConnection_WhenDisconnected_ThrowInvalidOperationExceptionOnReceive()
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            AgentLocalConnection agentLocalConnection = new AgentLocalConnection(communicationServer);
            GMLocalConnection gmLocalConnection = new GMLocalConnection(communicationServer);

            agentLocalConnection.Disconnect();
            gmLocalConnection.Disconnect();

            Assert.Throws<InvalidOperationException>(() => agentLocalConnection.Receive());
            Assert.Throws<InvalidOperationException>(() => gmLocalConnection.Receive());

            Assert.ThrowsAsync<InvalidOperationException>(() => agentLocalConnection.ReceiveAsync());
            Assert.ThrowsAsync<InvalidOperationException>(() => gmLocalConnection.ReceiveAsync());
        }

        [Fact]
        private void AgentLocalConnection_WhenMessageSent_ItIsDelivered()
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            AgentLocalConnection agentLocalConnection = new AgentLocalConnection(communicationServer);
            GMLocalConnection gmLocalConnection = new GMLocalConnection(communicationServer);

            Message message = new TestMessage(int.MaxValue);

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

            // Because of the determinism of LocalCommunicationServer i know that AgentId will be set to 1.
            Message message = new TestMessage(1,"");

            gmLocalConnection.Send(message);
            Message receivedMessage = agentLocalConnection.Receive();

            Assert.Equal(message, receivedMessage);
        }

        [Fact]
        private void LocalCommunicationServer_WhenSeveralMessagesSent_TheyAreDeliveredInRightOrder()
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            AgentLocalConnection agentLocalConnection = new AgentLocalConnection(communicationServer);
            GMLocalConnection gmLocalConnection = new GMLocalConnection(communicationServer);

            Message message1 = new TestMessage(int.MaxValue);
            Message message2 = new TestMessage(int.MaxValue);

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

            Message message1 = new TestMessage(int.MaxValue);
            Message message2 = new TestMessage(int.MaxValue);

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

            Message message1 = new TestMessage(int.MaxValue);
            Message message2 = new TestMessage(int.MaxValue);

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

        [Theory]
        [InlineData(5)]
        [InlineData(50)]
        public void GMLocalCommunication_WhenCollectionIsEmpty_TryTakeReturnsFalse(int waitTime)
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            GMLocalConnection gm = new GMLocalConnection(communicationServer);
            bool res = gm.TryReceive(out Message m, waitTime);
            res.ShouldBe(false);
            m.ShouldBe(null);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(50)]
        public void AgentLocalCommunication_WhenCollectionIsEmpty_TryTakeReturnsFalse(int waitTime)
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            AgentLocalConnection agent1 = new AgentLocalConnection(communicationServer);
            bool res = agent1.TryReceive(out Message m, waitTime);
            res.ShouldBe(false);
            m.ShouldBe(null);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(50)]
        public void GMLocalCommunication_WhenCollectionIsNotEmpty_TryTakeReturnsTrue(int waitTime)
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            GMLocalConnection gm = new GMLocalConnection(communicationServer);
            Message message = new TestMessage(0);
            gm.Messages.Add(message);
            bool res = gm.TryReceive(out Message m, waitTime);
            res.ShouldBe(true);
            m.ShouldBe(message);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(50)]
        public void AgentLocalCommunication_WhenCollectionIsNotEmpty_TryTakeReturnsTrue(int waitTime)
        {
            LocalCommunicationServer communicationServer = new LocalCommunicationServer();

            AgentLocalConnection agent1 = new AgentLocalConnection(communicationServer);
            Message message = new TestMessage(0);
            agent1.Messages.Add(message);
            bool res = agent1.TryReceive(out Message m, waitTime);
            res.ShouldBe(true);
            m.ShouldBe(message);
        }
    }
}
