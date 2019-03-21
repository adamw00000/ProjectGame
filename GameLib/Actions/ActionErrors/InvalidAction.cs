using GameLib.GameMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class InvalidAction : GameMasterMessage, IActionError
    {
        public InvalidAction(int agentId, int timestamp) : base(agentId, timestamp)
        {
        }

        public void Handle(Agent agent)
        {
            agent.HandleInvalidActionError(Timestamp);
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}
