using ConnectionLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.GameMessages
{
    public abstract class AgentMessage : Message
    {
        public AgentMessage(int agentId) : base(agentId)
        {
        }

        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }

        public abstract void Handle(GameMaster gameMaster);
    }
}
