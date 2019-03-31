namespace GameLib
{
    internal class ActionCommunicationRequest : GameMasterActionMessage
    {
        public readonly int requesterId;

        public ActionCommunicationRequest(int requesterId, int targetId, int timestamp) : base(targetId, timestamp)
        {
            this.requesterId = requesterId;
        }

        public override void Handle(Agent agent)
        {
            agent.HandleCommunicationRequest(requesterId, Timestamp);
        }
    }
}