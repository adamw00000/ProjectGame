using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionPickPieceResponse : GameMasterMessage, IActionResponse
    {
        public int WaitUntilTime { get; }

        public ActionPickPieceResponse(int agentId, int timestamp, int waitUntilTime) : base(agentId, timestamp)
        {
            this.WaitUntilTime = waitUntilTime;
        }

        public void Handle(Agent agent)
        {
            agent.HandlePickPieceResponse(Timestamp, WaitUntilTime);
        }

        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}