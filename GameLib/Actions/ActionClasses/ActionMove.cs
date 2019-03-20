using System;
using System.Collections.Generic;
using System.Text;
using GameLib;

namespace GameLib.Actions
{
    class ActionMove : IAction
    {
        private readonly MoveDirection moveDirection;
        public int AgentId { get; }

        public ActionMove(int agentId, MoveDirection moveDirection)
        {
            AgentId = agentId;
            this.moveDirection = moveDirection;
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.MoveAgent(AgentId, moveDirection);
        }
    }
}
