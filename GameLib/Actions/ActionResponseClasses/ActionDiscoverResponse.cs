using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionDiscoverResponse : IActionResponse
    {
        public ActionDiscoverResponse(int waitUntilTime, AgentField[,] closestPieces)
        {
            this.closestPieces = closestPieces;
            this.WaitUntilTime = waitUntilTime;
        }

        public int WaitUntilTime { get; }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }

        AgentField[,] closestPieces;
    }
}
