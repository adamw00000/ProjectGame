using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLib
{
    public class GMLocalConnection: LocalConnection, IConnection
    {
        public GMLocalConnection(LocalCommunicationServer communicationServer): base(communicationServer)
        {
            CommunicationServer.ConnectGM(this);
        }

        public override void Send(Message message)
        {
            if (!Connected)
            {
                throw new InvalidOperationException("Not connected");
            }

            CommunicationServer.SendMessage(this, message);
        }
    }
}
