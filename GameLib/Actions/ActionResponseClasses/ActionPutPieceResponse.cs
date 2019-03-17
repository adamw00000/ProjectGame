using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionPutPieceResponse : GameMasterMessage, IActionResponse
    {
        private readonly PutPieceResult putPieceResult;
        public int WaitUntilTime { get; }

        public ActionPutPieceResponse(int agentId, int timestamp, int waitUntilTime, PutPieceResult putPieceResult) : base(agentId, timestamp)
        {
            this.putPieceResult = putPieceResult;
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