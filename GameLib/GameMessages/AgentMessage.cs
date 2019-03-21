using ConnectionLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.GameMessages
{
    abstract class AgentMessage : Message
    {
        public AgentMessage(int agentId) : base(agentId)
        {

        }
    }
}
