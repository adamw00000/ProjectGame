using System;

namespace GameLib
{
    internal class ActionMakeMoveResponse : ActionResponseMessage
    {
        private readonly int closestPieceDistance;

        public ActionMakeMoveResponse(int agentId, int timestamp, int waitUntilTime, int closestPieceDistance, string messageId) : base(agentId, timestamp, waitUntilTime, messageId)
        {
            this.closestPieceDistance = closestPieceDistance;
        }

        public override void Handle(Agent agent)
        {
            agent.HandleMoveResponse(Timestamp, WaitUntilTime, closestPieceDistance, MessageId);
        }
    }
}