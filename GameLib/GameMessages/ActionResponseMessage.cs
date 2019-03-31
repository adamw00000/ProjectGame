namespace GameLib
{
    abstract public class ActionResponseMessage : GameMasterMessage
    {
        public readonly int WaitUntilTime;
        public readonly string MessageId;

        public ActionResponseMessage(int agentId, int timestamp, int waitUntilTime, string messageId) : base(agentId, timestamp) //MessageId temporrary "" because managing it is different task
        {
            WaitUntilTime = waitUntilTime;
            MessageId = messageId;
        }
    }
}