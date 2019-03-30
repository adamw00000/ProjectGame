using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionDestroyPieceMessage : ActionMessage
    {
        public ActionDestroyPieceMessage(int agentId, string messageId) : base(agentId, messageId)
        {

        }

        public override void Handle(GameMaster gameMaster)
        {
            gameMaster.DestroyPiece(AgentId);
        }
    }
}