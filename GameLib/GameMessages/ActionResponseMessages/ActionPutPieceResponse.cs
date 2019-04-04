using System;
namespace GameLib
{
    internal class ActionPutPieceResponse : ActionResponseMessage
    {
        public readonly PutPieceResult PutPieceResult;

        public ActionPutPieceResponse(int agentId, int timestamp, int waitUntilTime, PutPieceResult putPieceResult, string messageId) : base(agentId, timestamp, waitUntilTime, messageId)
        {
            this.PutPieceResult = putPieceResult;
        }

        public override void Handle(Agent agent)
        {
            agent.HandlePutPieceResponse(Timestamp, WaitUntilTime, PutPieceResult, MessageId);
        }

        public override string ToString()
        {
            return $"ActionPutPieceResponse (agentId: {AgentId}, messageId: {MessageId}, put piece result: {PutPieceResult})";
        }
    }
}