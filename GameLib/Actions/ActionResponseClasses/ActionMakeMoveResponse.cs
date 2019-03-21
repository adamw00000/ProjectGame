using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionMakeMoveResponse : GameMasterMessage, IActionResponse
    {
        public int WaitUntilTime { get; }
        private readonly int closestPieceDistance;

        public ActionMakeMoveResponse(int agentId, int timestamp, int waitUntilTime, int closestPieceDistance) : base(agentId, timestamp)
        {
            this.closestPieceDistance = closestPieceDistance;
            this.WaitUntilTime = waitUntilTime;
        }

        public void Handle(Agent agent)
        {
            agent.HandleMoveResponse(Timestamp, WaitUntilTime, closestPieceDistance);
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}