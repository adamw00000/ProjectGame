using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class ActionDiscovery : Action
    {
        public override void Execute(Agent agent)
        {
            agent.Discover(this);
        }

        public override string ToString()
        {
            return "ActionDiscovery";
        }
    }
}
