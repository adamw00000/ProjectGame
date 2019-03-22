using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionPickPiece : AgentMessage, IAction
    {
        public ActionPickPiece(int agentId) : base(agentId)
        {
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.PickPiece(AgentId);
        }
        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}