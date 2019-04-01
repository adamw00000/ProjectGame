using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionPutPieceResponse : ActionResponseMessage
    {
        public readonly PutPieceResult PutPieceResult;

        public ActionPutPieceResponse(int agentId, int timestamp, int waitUntilTime, PutPieceResult putPieceResult, string messageId) : base(agentId, timestamp, waitUntilTime, messageId) //MessageId temporrary "" because managing it is different task
        {
            this.PutPieceResult = putPieceResult;
        }

        public override void Handle(Agent agent)
        {
            agent.HandlePutPieceResponse(Timestamp, WaitUntilTime, PutPieceResult, MessageId);
        }
    }
}