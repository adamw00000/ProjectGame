using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionPutPiece : AgentMessage, IAction
    {
        public ActionPutPiece(int agentId, int timestamp) : base(agentId, timestamp)
        {
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.PutPiece(AgentId, Timestamp);
        }
        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}