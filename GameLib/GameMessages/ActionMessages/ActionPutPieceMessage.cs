namespace GameLib
{
    internal class ActionPutPieceMessage : ActionMessage
    {
        public ActionPutPieceMessage(int agentId, string messageId) : base(agentId, messageId)
        {
        }

        public override void Handle(GameMaster gameMaster)
        {
            gameMaster.PutPiece(AgentId, MessageId);
        }
    }
}