using System;

namespace GameLib.Actions
{
    internal class ActionPutPieceResponse : IActionResponse
    {
        public ActionPutPieceResponse(int waitUntilTime, PutPieceResult putPieceResult)
        {
            this.putPieceResult = putPieceResult;
            this.WaitUntilTime = waitUntilTime;
        }

        public int WaitUntilTime { get; }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }

        private readonly PutPieceResult putPieceResult;
    }
}