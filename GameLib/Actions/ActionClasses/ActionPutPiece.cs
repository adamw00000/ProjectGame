using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionPutPiece : AgentMessage, IAction
    {
        public ActionPutPiece(int agentId) : base(agentId)
        {
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.PutPiece(AgentId);
        }
        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}