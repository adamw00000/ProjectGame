using System;

namespace GameLib.Actions
{
    internal class ActionCommunicationRequestWithData : IAction
    {
        private readonly object data;

        public int AgentId { get; }
        public int TargetId { get; }

        public ActionCommunicationRequestWithData(int agentId, int targetId, object data)
        {
            AgentId = agentId;
            this.data = data;
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.CommunicationRequestWithData(AgentId, data);
        }
    }
}