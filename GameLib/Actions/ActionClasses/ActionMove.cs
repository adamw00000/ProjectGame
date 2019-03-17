using System;
using System.Collections.Generic;
using System.Text;
using GameLib;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    class ActionMove : AgentMessage, IAction
    {
        public readonly MoveDirection MoveDirection;

        public ActionMove(int agentId, MoveDirection moveDirection) : base(agentId)
        {
            this.MoveDirection = moveDirection;
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.MoveAgent(AgentId, MoveDirection);
        }
        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}
