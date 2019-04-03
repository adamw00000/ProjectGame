using System;
namespace GameLib
{
    internal class ActionCommunicationResponseWithData : ActionResponseMessage
    {
        private readonly object data;
        public readonly bool Agreement;
        public int SenderId;

        public ActionCommunicationResponseWithData(int agentId, int timestamp, int waitUntilTime, int senderId, bool agreement, object data, string messageId) : base(agentId, timestamp, waitUntilTime, messageId)
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