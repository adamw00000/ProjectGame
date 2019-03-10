using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionDestroyPiece : IAction
    {
        public ActionDestroyPiece(int agentId)
        {
            AgentId = agentId;
        }

        public int AgentId { get; }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.DestroyPiece(AgentId);
        }
    }
}
