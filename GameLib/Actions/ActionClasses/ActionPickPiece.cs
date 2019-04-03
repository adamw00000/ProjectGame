using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionPickPiece : AgentMessage, IAction
    {
        public ActionPickPiece(int agentId, int timestamp) : base(agentId, timestamp)
        {
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.PickUpPiece(AgentId, Timestamp);
        }
        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}