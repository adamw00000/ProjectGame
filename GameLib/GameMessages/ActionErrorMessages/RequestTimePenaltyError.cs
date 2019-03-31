using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    class RequestTimePenaltyError : ActionErrorMessage
    {
        public int RequestTimestamp { get; }

        public readonly int WaitUntilTime;

        public RequestTimePenaltyError(int agentId, int timestamp, int waitUntilTime, string messageId = "") : base(agentId, timestamp, messageId) //MessageId temporrary "" because managing it is different task
        {
            WaitUntilTime = waitUntilTime;
        }

        public override void Handle(Agent agent)
        {
            agent.HandleTimePenaltyError(Timestamp, WaitUntilTime, MessageId);
        }
    }
}
