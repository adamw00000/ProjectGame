using System;

namespace GameLib.Actions
{
    internal class ActionPutPieceResponse : IActionResponse
    {
        private readonly PutPieceResult putPieceResult;
        public int WaitUntilTime { get; }

        public ActionPutPieceResponse(int waitUntilTime, PutPieceResult putPieceResult)
        {
            this.putPieceResult = putPieceResult;
            this.WaitUntilTime = waitUntilTime;
        }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }
    }
}