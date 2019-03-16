using System;

namespace GameLib.Actions
{
    internal class ActionDiscoverResponse : IActionResponse
    {

        private readonly AgentField[,] closestPieces;
        public int WaitUntilTime { get; }

        public ActionDiscoverResponse(int waitUntilTime, AgentField[,] closestPieces)
        {
            this.closestPieces = closestPieces;
            this.WaitUntilTime = waitUntilTime;
        }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }
    }
}