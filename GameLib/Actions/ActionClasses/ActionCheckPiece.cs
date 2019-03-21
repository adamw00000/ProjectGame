using ConnectionLib;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionCheckPiece : AgentMessage, IAction
    {
        public ActionCheckPiece(int agentId) : base(agentId)
        {

        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.CheckPiece(AgentId);
        }

        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}