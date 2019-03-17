using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionDestroyPieceResponse : GameMasterMessage, IActionResponse
    {
        public int WaitUntilTime { get; }

        public ActionDestroyPieceResponse(int agentId, int timestamp, int waitUntilTime) : base(agentId, timestamp)
        {
            this.WaitUntilTime = waitUntilTime;
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