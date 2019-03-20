namespace GameLib.Actions
{
    internal class ActionCommunicationRequest : IGameMasterAction
    {
        private readonly int requesterId;

        public ActionCommunicationRequest(int requesterId)
        {
            this.requesterId = requesterId;
        }

        public void Handle(Agent agent)
        {
            agent.ServeCommunicationRequest(requesterId);
        }
    }
}