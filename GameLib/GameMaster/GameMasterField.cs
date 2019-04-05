using System;

namespace GameLib
{
    public struct GameMasterField: ICloneable
    {
        public int Distance { get; set; }

        public Piece Piece { get; set; }

        public bool HasPiece => Piece != null;
        public bool HasValidPiece => Piece?.IsValid == true;

        public bool IsGoal { get; set; }

        public object Clone()
        {
            GameMasterField field = new GameMasterField();
            field.Distance = Distance;
            field.Piece = (Piece)Piece?.Clone();
            field.IsGoal = IsGoal;
            return field;

        }

    }
}