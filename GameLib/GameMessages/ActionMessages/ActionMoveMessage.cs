using System;
using System.Collections.Generic;
using System.Text;
using GameLib;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    class ActionMoveMessage : ActionMessage
    {
        public readonly MoveDirection MoveDirection;

        public ActionMoveMessage(int agentId, MoveDirection moveDirection, string messageId) : base(agentId, messageId)
        {
            this.MoveDirection = moveDirection;
        }

        public override void Handle(GameMaster gameMaster)
        {
            gameMaster.MoveAgent(AgentId, MoveDirection, MessageId);
        }
    }
}
