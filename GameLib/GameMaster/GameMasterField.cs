using System;

namespace GameLib
{
    public struct GameMasterField
    {
        public int Distance { get; set; }

        public Piece Piece { get; set; }

        public bool HasPiece => Piece != null;
        public bool HasValidPiece => Piece != null && Piece.IsValid;
        
        public bool IsGoal { get; set; }
    }
}