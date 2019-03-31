using System;

namespace GameLib
{
    internal class ActionDiscoverResponse : ActionResponseMessage
    {
        private readonly DiscoveryResult closestPieces;

        public ActionDiscoverResponse(int agentId, int timestamp, int waitUntilTime, DiscoveryResult closestPieces, string messageId = "") : base(agentId, timestamp, waitUntilTime, messageId) //MessageId temporrary "" because managing it is different task
        {
            this.closestPieces = closestPieces;
        }

        public override void Handle(Agent agent)
        {
            agent.HandleDiscoverResponse(Timestamp, WaitUntilTime, closestPieces, MessageId);
        }
    }
}