using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionCommunicationAgreementWithData : IAction
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

        Object data;
    }
}
