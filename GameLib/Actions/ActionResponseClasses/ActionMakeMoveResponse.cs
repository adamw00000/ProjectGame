using System;

namespace GameLib.Actions
{
    internal class ActionMakeMoveResponse : IActionResponse
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

        private readonly int closestPieceDistance;
    }
}