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

        void Send<M>(M message);
        Task SendAsync<M>(M message);

        M Receive<M>();
        Task<M> ReceiveAsync<M>();
    }
}