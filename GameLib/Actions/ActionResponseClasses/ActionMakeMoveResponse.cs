using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionMakeMoveResponse : IActionResponse
    {
        public ActionMakeMoveResponse(int waitUntilTime, int closestPieceDistance)
        {
            this.closestPieceDistance = closestPieceDistance;
            this.WaitUntilTime = waitUntilTime;
        }

        public int WaitUntilTime { get; }


        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }

        int closestPieceDistance;
    }
}
