using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionPutPieceResponse : GameMasterMessage, IActionResponse
    {
        private readonly PutPieceResult PutPieceResult;
        public int WaitUntilTime { get; }

        public ActionPutPieceResponse(int agentId, int timestamp, int waitUntilTime, PutPieceResult putPieceResult) : base(agentId, timestamp)
        {
            this.PutPieceResult = putPieceResult;
            this.WaitUntilTime = waitUntilTime;
        }

        public void Handle(Agent agent)
        {
            agent.HandlePutPieceResponse(Timestamp, WaitUntilTime, PutPieceResult);
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}