using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionDestroyPieceResponse : ActionResponseMessage
    {
        public ActionDestroyPieceResponse(int agentId, int timestamp, int waitUntilTime, string messageId = "") : base(agentId, timestamp, waitUntilTime, messageId) //MessageId temporrary "" because managing it is different task
        {
        }

        public override void Handle(Agent agent)
        {
            agent.HandleDestroyPieceResponse(Timestamp, WaitUntilTime, MessageId);
        }
    }
}