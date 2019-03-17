using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionCheckPieceResponse : GameMasterMessage, IActionResponse
    {
        private readonly bool IsTrue;
        public int WaitUntilTime { get; }

        public ActionCheckPieceResponse(int agentId, int timestamp, int waitUntilTime, bool isTrue) : base(agentId, timestamp)
        {
            this.WaitUntilTime = waitUntilTime;
            this.IsTrue = isTrue;
        }
        
        public void Handle(Agent agent)
        {
            agent.HandleCheckPieceResponse(Timestamp, WaitUntilTime, IsTrue);
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}