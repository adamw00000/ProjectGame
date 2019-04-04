using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public abstract class Action
    {
        public abstract void Execute(Agent agent);
    }
}
