using System;

namespace GameLib.Actions
{
    internal class ActionDiscoverResponse : IActionResponse
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

        private readonly AgentField[,] closestPieces;
    }
}