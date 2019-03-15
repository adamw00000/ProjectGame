using System;

namespace GameLib.Actions
{
    internal class ActionCommunicationResponseWithData : IActionResponse
    {
        public ActionCommunicationResponseWithData(int waitUntilTime, object data)
        {
            this.WaitUntilTime = waitUntilTime;
            this.data = data;
        }

        public int WaitUntilTime { get; }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }

        private readonly object data;
    }
}