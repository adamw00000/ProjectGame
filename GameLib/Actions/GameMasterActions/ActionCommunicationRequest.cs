using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionCommunicationRequest : GameMasterMessage, IGameMasterAction
    {
        private readonly int requesterId;

        public ActionCommunicationRequest(int requesterId, int targetId, int timestamp) : base(targetId, timestamp)
        {
            this.requesterId = requesterId;
        }

        public void Handle(Agent agent)
        {
            agent.HandleCommunicationRequest(requesterId, Timestamp);
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}