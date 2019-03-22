using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionDestroyPiece : AgentMessage, IAction
    {
        public ActionDestroyPiece(int agentId) : base(agentId)
        {

        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.DestroyPiece(AgentId);
        }

        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}