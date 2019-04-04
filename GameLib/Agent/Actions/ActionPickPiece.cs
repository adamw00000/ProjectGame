using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class ActionPickPiece : Action
    {
        public override void Execute(Agent agent)
        {
            agent.PickPiece(this);
        }
    }
}
