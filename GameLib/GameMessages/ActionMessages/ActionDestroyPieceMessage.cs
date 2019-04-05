namespace GameLib
{
    internal class ActionDestroyPieceMessage : ActionMessage
    {
        public ActionDestroyPieceMessage(int agentId, string messageId) : base(agentId, messageId)
        {

        }

        public override void Handle(GameMaster gameMaster)
        {
            gameMaster.DestroyPiece(AgentId, MessageId);
        }

        public override string ToString()
        {
            return $"ActionDestroyPieceMessage (agentId: {AgentId}, messageId: {MessageId})";
        }
    }
}