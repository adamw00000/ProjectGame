using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionDestroyPiece : AgentMessage, IAction
    {
        public ActionDestroyPiece(int agentId, int timestamp) : base(agentId, timestamp)
        {

        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.DestroyPiece(AgentId, Timestamp);
        }

        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}