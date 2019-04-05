using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLib
{
    public abstract class LocalConnection: IConnection
    {
        #region Fields and Properties
        public BlockingCollection<Message> Messages { get; } = new BlockingCollection<Message>();
    
        protected LocalCommunicationServer CommunicationServer;
        public bool Connected { get; protected set; }
        #endregion

        #region Constructors
        public LocalConnection(LocalCommunicationServer communicationServer)
        {
            Connected = true;
            this.CommunicationServer = communicationServer;
        }
        #endregion

        #region Methods
        public void Disconnect()
        {
            Connected = false;
            CommunicationServer = null;
        }

        public Message Receive()
        {
            if (!Connected)
            {
                throw new InvalidOperationException("Not connected");
            }

            return Messages.Take();
        }

        public Task<Message> ReceiveAsync()
        {
            TaskCompletionSource<Message> taskCompletionSource = new TaskCompletionSource<Message>();
            taskCompletionSource.SetResult(Receive());
            return taskCompletionSource.Task;
        }

        public abstract void Send(Message message);

        public Task SendAsync(Message message)
        {
            Send(message);
            return Task.CompletedTask;
        }

        public bool TryReceive(out Message message, int timespan)
        {
            return Messages.TryTake(out message, timespan);
        }
        #endregion
    }
}
