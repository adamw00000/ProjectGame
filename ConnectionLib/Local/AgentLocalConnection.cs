using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLib
{
    public class AgentLocalConnection: LocalConnection, IConnection
    {
        public AgentLocalConnection(LocalCommunicationServer communicationServer): base(communicationServer)
        {
            CommunicationServer.ConnectAgent(this);
        }

        public override void Send(Message message)
        {
            if (!Connected)
            {
                throw new Exception("Not connected");
            }

            CommunicationServer.SendMessage(this, message);
        }
    }
}
