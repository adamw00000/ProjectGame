using GameLib.GameMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    abstract public class ActionErrorMessage : GameMasterMessage
    {
        public readonly string MessageId;

        public ActionErrorMessage(int agentId, int timestamp, string messageId) : base(agentId, timestamp)
        {
            MessageId = messageId;
        }
    }
}
