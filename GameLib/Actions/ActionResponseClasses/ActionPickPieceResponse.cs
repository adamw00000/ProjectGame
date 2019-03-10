using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionPickPieceResponse : IActionResponse
    {
        public ActionPickPieceResponse(int waitUntilTime)
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
