using ConnectionLib;
using System;

namespace GameLib
{
    public abstract class GameMasterMessage : Message
    {
        public GameMasterMessage(int agentId) : base(agentId)
        {
        }

        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }

        public abstract void Handle(Agent agent);
    }
}
