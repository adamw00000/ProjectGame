using System;

namespace GameLib.Actions
{
    internal class ActionDestroyPieceResponse : IActionResponse
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