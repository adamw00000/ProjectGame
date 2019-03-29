using GameLib.GameMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class InvalidMoveDirectionError : GameMasterMessage, IActionError
    {
        public int RequestTimestamp { get; }

        public InvalidMoveDirectionError(int agentId, int timestamp, int requestTimestamp) : base(agentId, timestamp)
        {
            RequestTimestamp = requestTimestamp;
        }

        public void Handle(Agent agent)
        {
            agent.HandleInvalidMoveDirectionError(Timestamp, RequestTimestamp);
        }

        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}
