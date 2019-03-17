using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionCommunicationResponseWithData : GameMasterMessage, IActionResponse
    {
        public int WaitUntilTime { get; }
        private readonly object data;
        public readonly bool Agreement;

        public ActionCommunicationResponseWithData(int agentId, int timestamp, int waitUntilTime, bool agreement, object data) : base(agentId, timestamp)
        {
            this.WaitUntilTime = waitUntilTime;
            this.data = data;
        }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }

    }
}