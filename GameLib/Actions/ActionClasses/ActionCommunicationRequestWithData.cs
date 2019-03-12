using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionCommunicationRequestWithData : IAction
    {
        public ActionCommunicationRequestWithData(int agentId, Object data)
        {
            AgentId = agentId;
        }

        public int AgentId { get; }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.CommunicationRequestWithData(AgentId, data);
        }
        
        Object data;
    }
}