using GameLib.GameMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class InvalidMoveDirectionError : GameMasterMessage, IActionError
    {
        public InvalidMoveDirectionError(int agentId, int timestamp) : base(agentId, timestamp)
        {
        }

        public void Handle(Agent agent)
        {
            agent.HandleInvalidMoveDirectionError(Timestamp);
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}
