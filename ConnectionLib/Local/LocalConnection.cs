using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLib
{
    public abstract class LocalConnection: IConnection
    {
        public BlockingCollection<object> Messages = new BlockingCollection<object>();
        public bool Connected { get; protected set; }


        public LocalConnection()
        {
            Connected = true;
        }

        public void Disconnect()
        {
            Connected = false;
        }

        public M Receive<M>()
        {
            if (!Connected)
            {
                throw new Exception("Not connected");
            }

            return (M)Messages.Take();
        }

        public Task<M> ReceiveAsync<M>()
        {
            TaskCompletionSource<M> taskCompletionSource = new TaskCompletionSource<M>();
            taskCompletionSource.SetResult(Receive<M>());
            return taskCompletionSource.Task;
        }

        public abstract void Send<M>(M message);

        public Task SendAsync<M>(M message)
        {
            Send(message);
            return Task.CompletedTask;
        }
    }
}
