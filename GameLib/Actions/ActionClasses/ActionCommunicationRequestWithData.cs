using System;

namespace GameLib.Actions
{
    internal class ActionCommunicationRequestWithData : IAction
    {
        public ActionCommunicationRequestWithData(int agentId, Object data)
        {
            AgentId = agentId;
            this.data = data;
        }

        public int AgentId { get; }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.CommunicationRequestWithData(AgentId, data);
        }

        private readonly Object data;
    }
}