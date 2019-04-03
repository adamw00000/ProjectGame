using GameLib.GameMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class InvalidAction : ActionErrorMessage
    {
        public InvalidAction(int agentId, int timestamp, string messageId) : base(agentId, timestamp, messageId)
        {
        }

        public override void Handle(Agent agent)
        {
            agent.HandleInvalidActionError(Timestamp, MessageId);
        }
    }
}
