using ConnectionLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.GameMessages
{
    abstract class AgentMessage : Message
    {
        public int Timestamp;

        public AgentMessage(int agentId, int timestamp) : base(agentId)
        {
            this.Timestamp = timestamp;
        }
    }
}
