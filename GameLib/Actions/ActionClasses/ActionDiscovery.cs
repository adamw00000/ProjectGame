using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionDiscovery : AgentMessage, IAction
    {
        public ActionDiscovery(int agentId) : base(agentId)
        {
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.Discover(AgentId);
        }
        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}