using GameLib.GameMessages;

namespace GameLib.Actions
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
    }
}