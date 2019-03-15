using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLib
{
    public interface IConnection
    {
        bool Connected { get; }

        void Disconnect();

        void Send(Message message);
        Task SendAsync(Message message);

        Message Receive();
        Task<Message> ReceiveAsync();
    }
}