using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionPutPieceResponse : IActionResponse
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

        PutPieceResult putPieceResult;
    }
}
