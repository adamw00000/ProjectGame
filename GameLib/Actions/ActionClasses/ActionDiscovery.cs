using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionDiscovery : IAction
    {
        public ActionDiscovery(int agentId)
        {
            AgentId = agentId;
        }

        public int AgentId { get; }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.Discover(AgentId);
        }
    }
}
