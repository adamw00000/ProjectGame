using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLib
{
    public class AgentLocalConnection: LocalConnection, IConnection
    {
        public AgentLocalConnection(): base()
        {
            LocalCommunicationServer.ConnectAgent(this);
        }

        public override void Send<M>(M message)
        {
            if (!Connected)
            {
                throw new Exception("Not connected");
            }

            LocalCommunicationServer.SendMessage(this, message);
        }
    }
}
