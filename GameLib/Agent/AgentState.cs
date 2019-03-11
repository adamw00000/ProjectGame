using System;
using System.Collections.Generic;
using System.Text;
using static GameLib.Exceptions;

namespace GameLib
{
    public class AgentState
    {
        public AgentBoard Board { get; private set; }

        public bool HoldsPiece = false;
        public PieceState PieceState = PieceState.Unknown;
        public (int X, int Y) Position;

        public AgentState()
        {

        }

        // Ta metoda została wyodrębniona z konstruktora bo nie chcemy tworzyć całego obiektu w momencie rozpoczęcia gry
        // tylko uzupełnić planszę (która zależy od infromacji otrzymanych na początku gry.
        public void Setup(GameRules rules)
        {
            Position = (rules.AgentStartX, rules.AgentStartY);
            Board = new AgentBoard(rules);
        }

        public void Move(Direction direction, int distance)
        {
            var oldPosition = Position;

            switch(direction)
            {
                case Direction.Left:
                    Position.Y--;
                    break;
                case Direction.Right:
                    Position.Y++;
                    break;
                case Direction.Up:
                    Position.X--;
                    break;
                case Direction.Down:
                    Position.X++;
                    break;
            }

            if (Position.X >= Board.Width || Position.X < 0 ||
                Position.Y >= Board.Height || Position.Y < 0)
            {
                Position = oldPosition;
                throw new InvalidMoveException();
            }

            Board.SetDistance(Position.X, Position.Y, distance);
        }

        public void PickUpPiece()
        {
            if (HoldsPiece)
                throw new PieceOperationException("Picking up piece when agent has one already");

            HoldsPiece = true;
            PieceState = PieceState.Unknown;
        }

        public void SetPieceState(PieceState newState)
        {
            PieceState = newState;
        }

        public void PlaceOrDestroyPiece()
        {
            if (!HoldsPiece)
                throw new PieceOperationException("Placing or destroying piece when agent doesn't have it");

            HoldsPiece = false;
            PieceState = PieceState.Unknown;
        }

        public void Discover(AgentDiscoveryResult discoveryResult)
        {
            if (!discoveryResult.IsValid(Board))
                throw new InvalidDiscoveryResultException();

            Board.ApplyDiscoveryResult(discoveryResult);
        }

        public void ApplyCommunicationResult(CommunicationResult communicationResult)
        {
            if (!communicationResult.IsValid(Board))
                throw new InvalidCommunicationResultException();

            Board.ApplyCommunicationResult(communicationResult.Board);
        }
    }

    public enum PieceState
    {
        Valid,
        Invalid,
        Unknown
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}