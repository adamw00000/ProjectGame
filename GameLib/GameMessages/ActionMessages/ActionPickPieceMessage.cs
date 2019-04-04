namespace GameLib
{
    internal class ActionPickPieceMessage : ActionMessage
    {
        public ActionPickPieceMessage(int agentId, string messageId) : base(agentId, messageId)
        {
        }

        public override void Handle(GameMaster gameMaster)
        {
            gameMaster.PickPiece(AgentId, MessageId);
        }

        public override string ToString()
        {
            return $"ActionPickPieceMessage (agentId: {AgentId}, messageId: {MessageId})";
        }
    }
}