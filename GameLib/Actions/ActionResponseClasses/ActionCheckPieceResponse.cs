using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionCheckPieceResponse : GameMasterMessage, IActionResponse
    {
        private readonly bool isCorrect;
        public int WaitUntilTime { get; }

        public ActionCheckPieceResponse(int agentId, int timestamp, int waitUntilTime, bool isCorrect) : base(agentId, timestamp)
        {
            this.WaitUntilTime = waitUntilTime;
            this.isCorrect = isCorrect;
        }
        
        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}