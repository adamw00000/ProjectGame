using ConnectionLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.GameMessages
{
    public abstract class GameMasterMessage : Message
    {
        public readonly int Timestamp;

        public GameMasterMessage(int agentId, int timestamp) : base(agentId)
        {
            Timestamp = timestamp;
        }

        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }

        public abstract void Handle(Agent agent);
    }
}
