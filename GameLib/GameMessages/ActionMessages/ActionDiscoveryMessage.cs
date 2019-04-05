namespace GameLib
{
    internal class ActionDiscoveryMessage : ActionMessage
    {
        public ActionDiscoveryMessage(int agentId, string messageId) : base(agentId, messageId)
        {
        }

        public override void Handle(GameMaster gameMaster)
        {
            gameMaster.Discover(AgentId, MessageId);
        }

        public override string ToString()
        {
            return $"ActionDiscoveryMessage (agentId: {AgentId}, messageId: {MessageId})";
        }
    }
}