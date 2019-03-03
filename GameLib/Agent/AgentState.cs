using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class AgentState
    {
        public AgentBoard Board { get; private set; }

        public bool HoldsPiece = false;
        public PieceState PieceState = PieceState.Unknown;
        public (int X, int Y) Position; // tak wiem, krotka jest nie fajna, można to zmienić, na razie jest tak

        public AgentState()
        {

        }

        // Ta metoda została wyodrębniona z konstruktora bo nie chcemy tworzyć całego obiektu w momencie rozpoczęcia gry
        // tylko uzupełnić planszę (która zależy od infromacji otrzymanych na początku gry.
        public void Setup(GameRules rules)
        {
            Board = new AgentBoard(rules);
            // uzupełnić Position itd

            throw new NotImplementedException();
        }
    }

    public enum PieceState
    {
        Valid,
        Invalid,
        Unknown
    }
}