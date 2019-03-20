using System;

namespace GameLib.Actions
{
    internal class ActionDestroyPieceResponse : IActionResponse
    {
        public int WaitUntilTime { get; }

        public ActionDestroyPieceResponse(int waitUntilTime)
        {
            this.WaitUntilTime = waitUntilTime;
        }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }
    }
}