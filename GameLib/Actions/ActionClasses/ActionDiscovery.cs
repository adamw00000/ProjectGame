namespace GameLib.Actions
{
    internal class ActionDiscovery : IAction
    {
        public ActionDiscovery(int agentId)
        {
            AgentId = agentId;
        }

        public int AgentId { get; }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.Discover(AgentId);
        }
    }
}