using System;
using System.Linq;

namespace GameLib
{
    internal class ActionDiscoverResponse : ActionResponseMessage
    {
        private readonly DiscoveryResult closestPieces;

        public ActionDiscoverResponse(int agentId, int timestamp, int waitUntilTime, DiscoveryResult closestPieces, string messageId) : base(agentId, timestamp, waitUntilTime, messageId)
        {
            this.closestPieces = closestPieces;
        }

        public override void Handle(Agent agent)
        {
            agent.HandleDiscoverResponse(Timestamp, WaitUntilTime, closestPieces, MessageId);
        }

        public override string ToString()
        {
            return $"ActionDiscoverResponse (agentId: {AgentId}, messageId: {MessageId}, " +
                $"discovery result: {closestPieces.Fields.Aggregate("", (s, tuple) => s + $"({tuple.x},{tuple.y},{tuple.distance}) ")})";
        }
    }
}