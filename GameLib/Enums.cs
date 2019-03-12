using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public enum MoveDirection
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    public enum PutPieceResult
    {
        PIECE_IN_TASK_AREA = 0,
        PIECE_GOAL_REALIZED = 1,
        PIECE_GOAL_UNREALIZED = 2,
        PIECE_WAS_FAKE = 3
    }

    public enum Team
    {
        Blue = 0,
        Red = 1
    }

    public enum PieceState //What agent knows about his piece
    {
        Valid,
        Invalid,
        Unknown
    }

    public enum FieldState //What agent knows about fields on the board
    {
        DiscoveredGoal,
        DiscoveredNotGoal,
        Unknown,
        NA // Not Applicable (in task area)
    }
}
