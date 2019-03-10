using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionPickPiece : IAction
    {
        public ActionPickPiece(int agentId)
        {

        }

        public int AgentId { get; }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.PickPiece(AgentId);
        }
    }
}
