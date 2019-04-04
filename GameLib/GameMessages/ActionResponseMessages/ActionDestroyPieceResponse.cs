namespace GameLib
{
    internal class ActionDestroyPieceResponse : ActionResponseMessage
    {
        public ActionDestroyPieceResponse(int agentId, int timestamp, int waitUntilTime, string messageId) : base(agentId, timestamp, waitUntilTime, messageId)
        {
        }

        public override void Handle(Agent agent)
        {
            agent.HandleDestroyPieceResponse(Timestamp, WaitUntilTime, MessageId);
        }

        public override string ToString()
        {
            return $"ActionDestroyPieceResponse (agentId: {AgentId}, messageId: {MessageId})";
        }
    }
}