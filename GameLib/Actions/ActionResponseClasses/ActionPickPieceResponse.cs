using System;

namespace GameLib.Actions
{
    internal class ActionPickPieceResponse : IActionResponse
    {
        public int WaitUntilTime { get; }

        public ActionPickPieceResponse(int waitUntilTime)
        {
            this.WaitUntilTime = waitUntilTime;
        }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }
    }
}