using ConnectionLib;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionCheckPieceMessage : ActionMessage
    {
        public ActionCheckPieceMessage(int agentId, string messageId) : base(agentId, messageId)
        {

        }

        public override void Handle(GameMaster gameMaster)
        {
            gameMaster.CheckPiece(AgentId);
        }
    }
}