using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectionLib
{
    public abstract class Message
    {
        public int AgentId { get; set; }

        public Message(int agentId)
        {
            this.AgentId = agentId;
        }

        public abstract void Handle(object handler);
        // TODO Think about the rest of the class
    }
}
