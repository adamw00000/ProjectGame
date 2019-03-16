using System;

namespace GameLib.Actions
{
    internal class ActionCommunicationAgreementWithData : IAction
    {
        private readonly object data;

        public int AgentId { get; }
        public int TargetId { get; }
        public bool AcceptsCommunication { get; }

        public ActionCommunicationAgreementWithData(int agentId, int targetId, bool acceptsCommunication, object data = null)
        {
            AgentId = agentId;
            TargetId = targetId;
            AcceptsCommunication = acceptsCommunication;
            this.data = data;
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.CommunicationAgreementWithData(AgentId, data);
        }
    }
}