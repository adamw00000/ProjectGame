using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionPickPieceResponse : ActionResponseMessage
    {

        public ActionPickPieceResponse(int agentId, int timestamp, int waitUntilTime, string messageId = "") : base(agentId, timestamp, waitUntilTime, messageId) //MessageId temporrary "" because managing it is different task
        {

        }

        public override void Handle(Agent agent)
        {
            agent.HandlePickPieceResponse(Timestamp, WaitUntilTime, MessageId);
        }
    }
}