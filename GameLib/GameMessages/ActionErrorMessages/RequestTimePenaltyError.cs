using GameLib.GameMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class RequestTimePenaltyError : ActionErrorMessage
    {
        public int RequestTimestamp { get; }

        public readonly int WaitUntilTime;

        public RequestTimePenaltyError(int agentId, int timestamp, int waitUntilTime, string messageId) : base(agentId, timestamp, messageId)
        {
            WaitUntilTime = waitUntilTime;
        }

        public override void Handle(Agent agent)
        {
            agent.HandleTimePenaltyError(Timestamp, WaitUntilTime, MessageId);
        }
    }
}
