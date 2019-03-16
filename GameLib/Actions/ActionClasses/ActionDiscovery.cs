namespace GameLib.Actions
{
    internal class ActionDiscovery : IAction
    {
        public int AgentId { get; }

        public ActionDiscovery(int agentId)
        {
            AgentId = agentId;
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.Discover(AgentId);
        }
    }
}