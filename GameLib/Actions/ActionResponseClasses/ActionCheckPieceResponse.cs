using System;

namespace GameLib.Actions
{
    internal class ActionCheckPieceResponse : IActionResponse
    {
        public ActionCheckPieceResponse(int waitUntilTime, bool isCorrect)
        {
            this.WaitUntilTime = waitUntilTime;
            this.isCorrect = isCorrect;
        }

        public int WaitUntilTime { get; }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }

        private readonly bool isCorrect;
    }
}