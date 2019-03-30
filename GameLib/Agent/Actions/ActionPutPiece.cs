using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class ActionPutPiece : Action
    {
        public override void Execute(Agent agent)
        {
            agent.PutPiece();
        }
    }
}
