using System;
namespace GameLib
{
    internal class ActionPickPieceResponse : ActionResponseMessage
    {

        public ActionPickPieceResponse(int agentId, int timestamp, int waitUntilTime, string messageId) : base(agentId, timestamp, waitUntilTime, messageId)
        {

        }

        public override void Handle(Agent agent)
        {
            agent.HandlePickPieceResponse(Timestamp, WaitUntilTime, MessageId);
        }
    }
}