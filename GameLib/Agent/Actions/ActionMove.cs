using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class ActionMove : Action
    {
        public MoveDirection Direction;

        public ActionMove(MoveDirection direction)
        {
            Direction = direction;
        }

        public override void Execute(Agent agent)
        {
            agent.Move(this);
        }

        public override string ToString()
        {
            return $"ActionMove (direction: {Direction})";
        }
    }
}
