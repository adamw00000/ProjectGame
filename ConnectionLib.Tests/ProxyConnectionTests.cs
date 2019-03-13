using System;
using Xunit;

using ConnectionLib;

namespace ConnectionLib.Tests
{
    class ExampleMessage
    {
        int AgentId { get; set; }
        string Text;

        public ExampleMessage(int id, string text)
        {
            AgentId = id;
            Text = text;
        }
    }

    public class ProxyConnectionTests
    {
        [Fact]
        public void ConnectionTest1()
        {
            LocalCommunicationServer.Clear();

            GMLocalConnection gm = new GMLocalConnection();
            AgentLocalConnection agent = new AgentLocalConnection();

            ExampleMessage original = new ExampleMessage(1, "Przykladowy tekst");
            agent.Send(original);

            ExampleMessage recived = gm.Receive<ExampleMessage>();

            Assert.Equal(original, recived);
        }

        [Fact]
        public void ConnectionTest2()
        {
            LocalCommunicationServer.Clear();

            GMLocalConnection gm = new GMLocalConnection();
            AgentLocalConnection agent = new AgentLocalConnection();

            ExampleMessage original = new ExampleMessage(1, "Przykladowy tekst");
            agent.Send(original);

            ExampleMessage recived1 = gm.Receive<ExampleMessage>();
            gm.Send(recived1);

            ExampleMessage recived2 = agent.Receive<ExampleMessage>();

            Assert.Equal(original, recived1);
            Assert.Equal(recived1, recived2);
        }
    }
}
