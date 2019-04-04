using System;

namespace GameLib
{
    internal class ActionMoveResponse : ActionResponseMessage
    {
        private readonly int closestPieceDistance;

        public ActionMoveResponse(int agentId, int timestamp, int waitUntilTime, int closestPieceDistance, string messageId) : base(agentId, timestamp, waitUntilTime, messageId)
        {
            this.closestPieceDistance = closestPieceDistance;
        }

        public override void Handle(Agent agent)
        {
            agent.HandleMoveResponse(Timestamp, WaitUntilTime, closestPieceDistance, MessageId);
        }
    }
}