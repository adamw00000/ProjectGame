using GameLib.GameMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class RequestTimePenaltyError : GameMasterMessage, IActionError
    {
        public readonly int WaitUntilTime;
        public RequestTimePenaltyError(int agentId, int timestamp, int waitUntilTime) : base(agentId, timestamp)
        {
            WaitUntilTime = waitUntilTime;
        }

        public void Handle(Agent agent)
        {
            agent.HandleTimePenaltyError(Timestamp, WaitUntilTime);
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}
