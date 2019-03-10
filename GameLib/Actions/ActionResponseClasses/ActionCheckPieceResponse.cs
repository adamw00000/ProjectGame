using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionCheckPieceResponse : IActionResponse
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

        bool isCorrect;
    }
}
