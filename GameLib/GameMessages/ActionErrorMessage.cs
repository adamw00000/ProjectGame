using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    abstract public class ActionErrorMessage : GameMasterGameMessage
    {
        public readonly string MessageId;

        public ActionErrorMessage(int agentId, int timestamp, string messageId) : base(agentId, timestamp)
        {
            MessageId = messageId;
        }
    }
}
