using GameLib.GameMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class RequestTimePenaltyError : GameMasterMessage, IActionError
    {
        public int RequestTimestamp { get; }

        public readonly int WaitUntilTime;

        public RequestTimePenaltyError(int agentId, int timestamp, int waitUntilTime, int requestTimestamp) : base(agentId, timestamp)
        {
            WaitUntilTime = waitUntilTime;
            RequestTimestamp = requestTimestamp;
        }

        public void Handle(Agent agent)
        {
            agent.HandleTimePenaltyError(Timestamp, RequestTimestamp, WaitUntilTime);
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}
