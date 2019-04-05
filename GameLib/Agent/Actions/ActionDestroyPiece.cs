using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class ActionDestroyPiece : Action
    {
        public override void Execute(Agent agent)
        {
            agent.DestroyPiece(this);
        }

        public override string ToString()
        {
            return "ActionDestroyPiece";
        }
    }
}
