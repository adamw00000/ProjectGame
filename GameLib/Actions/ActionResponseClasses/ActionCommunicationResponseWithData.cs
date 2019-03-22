using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionCommunicationResponseWithData : GameMasterMessage, IActionResponse
    {
        public int WaitUntilTime { get; }
        private readonly object data;
        public readonly bool Agreement;
        public int SenderId;

        public ActionCommunicationResponseWithData(int agentId, int timestamp, int waitUntilTime, int senderId, bool agreement, object data) : base(agentId, timestamp)
        {
            this.WaitUntilTime = waitUntilTime;
            this.data = data;
            this.SenderId = senderId;
        }

        public void Handle(Agent agent)
        {
            agent.HandleCommunicationResponse(Timestamp, WaitUntilTime, SenderId, Agreement, data);
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }

    }
}