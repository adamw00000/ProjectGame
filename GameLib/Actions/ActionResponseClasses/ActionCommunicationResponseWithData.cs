using System;

namespace GameLib.Actions
{
    internal class ActionCommunicationResponseWithData : IActionResponse
    {
        public int WaitUntilTime { get; }
        private readonly object data;

        public ActionCommunicationResponseWithData(int waitUntilTime, object data)
        {
            this.WaitUntilTime = waitUntilTime;
            this.data = data;
        }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }

    }
}