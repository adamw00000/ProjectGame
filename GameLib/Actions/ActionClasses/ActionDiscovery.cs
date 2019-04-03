using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionDiscovery : AgentMessage, IAction
    {
        public ActionDiscovery(int agentId, int timestamp) : base(agentId, timestamp)
        {
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.Discover(AgentId, Timestamp);
        }
        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}