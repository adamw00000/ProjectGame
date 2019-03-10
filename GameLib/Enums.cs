using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public enum MoveDirection
    {
        UP = 0,
        RIGHT = 1,
        DOWN = 2,
        LEFT = 3
    }

    public enum PutPieceResult
    {
        PIECE_IN_TASK_AREA = 0,
        PIECE_GOAL_REALIZED = 1,
        PIECE_GOAL_UNREALIZED = 2,
        PIECE_WAS_FAKE = 3
    }
}
