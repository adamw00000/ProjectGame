using System;
namespace GameLib
{
    internal class ActionCheckPieceResponse : ActionResponseMessage
    {
        public readonly bool IsValid;

        public ActionCheckPieceResponse(int agentId, int timestamp, int waitUntilTime, bool isValid, string messageId) : base(agentId, timestamp, waitUntilTime, messageId)
        {
            this.IsValid = isValid;
        }
        
        public override void Handle(Agent agent)
        {
            agent.HandleCheckPieceResponse(Timestamp, WaitUntilTime, IsValid, MessageId);
        }
    }
}