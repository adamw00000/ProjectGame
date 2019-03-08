using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConnectionLib
{
    public class ProxyConnection: IConnection
    {
        public enum ConnectionFrom
        {
            Client,
            GM
        }

        #region Statics
        private static ConcurrentDictionary<int, ProxyConnection> agents = new ConcurrentDictionary<int, ProxyConnection>();
        private static ProxyConnection gameMaster;
        #endregion

        #region Private fields
        private ConcurrentQueue<object> incomingMessages;
        #endregion

        #region Public properties
        public bool Connected { get; private set; } = false;
        #endregion

        #region Constructors
        public ProxyConnection(ConnectionFrom connectionFrom)
        {

            Connected = true;
        }
        #endregion

        #region Public methods
        public void Disconnect()
        {
            Connected = false;
        }

        public void Send<M>(M message)
        {
            if (!Connected)
            {
                throw new InvalidOperationException();
            }

            outgoingMessages.Enqueue(message);
        }

        public M Receive<M>()
        {
            if (!Connected)
            {
                throw new InvalidOperationException();
            }

            object message = null;
            while (!incomingMessages.TryDequeue(out message))
            { } // tu jest problem jeśli nigdy się nie uda zdjąć
            return (M)message;
        }

        public async Task SendAsync<M>(M message)
        {
            Send(message);
        }

        public async Task<M> ReceiveAsync<M>()
        {
            return Receive<M>();
        }
        #endregion
    }
}