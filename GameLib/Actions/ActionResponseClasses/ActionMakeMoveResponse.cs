using System;

namespace GameLib.Actions
{
    internal class ActionMakeMoveResponse : IActionResponse
    {
        public int WaitUntilTime { get; }
        private readonly int closestPieceDistance;

        public ActionMakeMoveResponse(int waitUntilTime, int closestPieceDistance)
        {
            this.closestPieceDistance = closestPieceDistance;
            this.WaitUntilTime = waitUntilTime;
        }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }
    }
}