using System;
using System.Collections.Generic;
using System.Text;
using GameLib;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    class ActionMove : AgentMessage, IAction
    {
        private readonly MoveDirection moveDirection;

        public ActionMove(int agentId, MoveDirection moveDirection) : base(agentId)
        {
            this.moveDirection = moveDirection;
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.MoveAgent(AgentId, moveDirection);
        }
        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}
