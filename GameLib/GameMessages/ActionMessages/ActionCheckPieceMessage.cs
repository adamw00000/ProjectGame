using ConnectionLib;
namespace GameLib
{
    internal class ActionCheckPieceMessage : ActionMessage
    {
        public ActionCheckPieceMessage(int agentId, string messageId) : base(agentId, messageId)
        {

        }

        public override void Handle(GameMaster gameMaster)
        {
            gameMaster.CheckPiece(AgentId, MessageId);
        }

        public override string ToString()
        {
            return $"ActionCheckPieceMessage (agentId: {AgentId}, messageId: {MessageId})";
        }
    }
}