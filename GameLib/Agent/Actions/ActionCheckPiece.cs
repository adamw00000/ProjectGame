using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class ActionCheckPiece : Action
    {
        public override void Execute(Agent agent)
        {
            agent.CheckPiece();
        }
    }
}
