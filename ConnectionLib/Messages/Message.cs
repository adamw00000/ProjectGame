using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectionLib
{
    public class Message
    {
        public int AgentId { get; set; }


        public Message(int agentId)
        {
            this.AgentId = agentId;
        }

        // TODO Think about the rest of the class
    }
}
