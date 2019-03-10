using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionDestroyPieceResponse : IActionResponse
    {
        public ActionDestroyPieceResponse(int waitUntilTime)
        {
            this.WaitUntilTime = waitUntilTime;
        }

        public int WaitUntilTime { get; }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }
    }
}
