using System;
using System.Threading.Tasks;

namespace ConnectionLib
{
    public class ProxyConnection: IConnection
    {
        #region Public properties
        public bool Connected => throw new NotImplementedException();
        #endregion

        #region Constructors
        public ProxyConnection()
        {

        }
        #endregion

        #region Public methods
        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Send<M>(M message)
        {
            throw new NotImplementedException();
        }

        public M Receive<M>()
        {
            throw new NotImplementedException();
        }

        public Task SendAsync<M>(M message)
        {
            throw new NotImplementedException();
        }

        public Task<M> ReceiveAsync<M>()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}