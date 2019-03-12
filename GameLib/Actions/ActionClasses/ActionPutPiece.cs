using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionPutPiece : IAction
    {
        public ActionPutPiece(int agentId)
        {
            AgentId = agentId;
        }

        public int AgentId { get; }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.PutPiece(AgentId);
        }
    }
}
