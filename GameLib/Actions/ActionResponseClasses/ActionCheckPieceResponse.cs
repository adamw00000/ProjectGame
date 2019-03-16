using System;

namespace GameLib.Actions
{
    internal class ActionCheckPieceResponse : IActionResponse
    {
        private readonly bool isCorrect;
        public int WaitUntilTime { get; }

        public ActionCheckPieceResponse(int waitUntilTime, bool isCorrect)
        {
            this.WaitUntilTime = waitUntilTime;
            this.isCorrect = isCorrect;
        }
        
        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }
    }
}