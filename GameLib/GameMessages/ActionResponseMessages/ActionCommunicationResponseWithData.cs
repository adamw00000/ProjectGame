using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionCommunicationResponseWithData : ActionResponseMessage
    {
        private readonly object data;
        public readonly bool Agreement;
        public int SenderId;

        public ActionCommunicationResponseWithData(int agentId, int timestamp, int waitUntilTime, int senderId, bool agreement, object data, string messageId) : base(agentId, timestamp, waitUntilTime, messageId) //MessageId temporrary "" because managing it is different task
        {
            this.data = data;
            this.SenderId = senderId;
            this.Agreement = agreement;
        }

        public override void Handle(Agent agent)
        {
            agent.HandleCommunicationResponse(Timestamp, WaitUntilTime, SenderId, Agreement, data, MessageId);
        }
    }
}