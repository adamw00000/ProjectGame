using System;

namespace GameLib
{
    public struct GameMasterField
    {
        public int Distance { get; set; }

        public Piece Piece { get; set; }

        public bool IsGoal => Piece != null && Piece.IsValid; //Nazwalbym to HasValidPiece
        //IsGoal powinno byc zarezerwowane dla pol w Goal Area, ktore sa golami
        public bool HasPiece => Piece != null;

        //public Agent // itd.... (w gestii implementującego)
    }
}