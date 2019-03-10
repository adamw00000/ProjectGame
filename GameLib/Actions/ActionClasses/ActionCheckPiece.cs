using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionCheckPiece : IAction
    {
        public ActionCheckPiece(int agentId)
        {
            AgentId = agentId;
        }

        public int AgentId { get; }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.CheckPiece(AgentId);
        }
    }
}
