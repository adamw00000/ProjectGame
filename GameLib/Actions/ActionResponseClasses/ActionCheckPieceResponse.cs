using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionCheckPieceResponse : GameMasterMessage, IActionResponse
    {
        private readonly bool IsValid;
        public int WaitUntilTime { get; }

        public ActionCheckPieceResponse(int agentId, int timestamp, int waitUntilTime, bool isValid) : base(agentId, timestamp)
        {
            this.WaitUntilTime = waitUntilTime;
            this.IsValid = isValid;
        }
        
        public void Handle(Agent agent)
        {
            agent.HandleCheckPieceResponse(Timestamp, WaitUntilTime, IsValid);
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}