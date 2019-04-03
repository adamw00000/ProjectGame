namespace GameLib
{
    public abstract class ActionMessage : AgentMessage
    {
        public readonly string MessageId;

        public ActionMessage(int agentId, string messageId) : base(agentId)
        {
            MessageId = messageId;
        }
    }
}