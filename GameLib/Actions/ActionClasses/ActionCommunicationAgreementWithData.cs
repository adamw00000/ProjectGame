using System;

namespace GameLib.Actions
{
    internal class ActionCommunicationAgreementWithData : IAction
    {
        public ActionCommunicationAgreementWithData(int agentId, Object data)
        {
            AgentId = agentId;
            this.data = data;
        }

        public int AgentId { get; }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.CommunicationAgreementWithData(AgentId, data);
        }

        private readonly Object data;
    }
}