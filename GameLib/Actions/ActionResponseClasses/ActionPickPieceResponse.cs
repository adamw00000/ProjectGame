using System;

namespace GameLib.Actions
{
    internal class ActionPickPieceResponse : IActionResponse
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