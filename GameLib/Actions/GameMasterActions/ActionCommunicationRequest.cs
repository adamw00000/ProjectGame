namespace GameLib.Actions
{
    internal class ActionCommunicationRequest : IGameMasterAction
    {
        public ActionCommunicationRequest(int requesterId)
        {
            this.requesterId = requesterId;
        }

        public void Handle(Agent agent)
        {
            agent.ServeCommunicationRequest(requesterId);
        }

        private readonly int requesterId;
    }
}