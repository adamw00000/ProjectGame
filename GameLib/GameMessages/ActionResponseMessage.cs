namespace GameLib
{
    abstract public class ActionResponseMessage : GameMasterGameMessage
    {
        public readonly int WaitUntilTime;
        public readonly string MessageId;

        public ActionResponseMessage(int agentId, int timestamp, int waitUntilTime, string messageId) : base(agentId, timestamp)
        {
            WaitUntilTime = waitUntilTime;
            MessageId = messageId;
        }
    }
}