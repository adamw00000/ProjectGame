using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameLib
{
    public class GameMasterState
    {
        public readonly bool GameEnded = false;
        private readonly double validPieceProbability;
        private readonly int maxPiecesOnBoard;

        public readonly GameMasterBoard Board;
        public Dictionary<int, PlayerState> PlayerStates = new Dictionary<int, PlayerState>();

        public GameMasterState(GameRules rules)
        {
            Board = new GameMasterBoard(rules);
            validPieceProbability = 1 - rules.BadPieceProbability;
            maxPiecesOnBoard = rules.MaxPiecesOnBoard;
        }

        public void GeneratePiece()
        {
            if (Board.PieceCount < maxPiecesOnBoard)
                Board.GeneratePiece(validPieceProbability);
        }
        public void GeneratePieceOnCoordinates(int x, int y) //do testow!
        {
            if (Board.PieceCount < maxPiecesOnBoard)
                Board.GeneratePieceOnCoordinates(x, y, validPieceProbability);
        }

        public void Move(int playerId, Direction direction)
        {
            if (!PlayerStates[playerId].IsEligibleForAction)
                throw new DelayException();

            var newPosition = PlayerStates[playerId].Position;

            switch (direction)
            {
                case Direction.Up:
                    newPosition.Y--;
                    break;
                case Direction.Down:
                    newPosition.Y++;
                    break;
                case Direction.Left:
                    newPosition.X--;
                    break;
                case Direction.Right:
                    newPosition.X++;
                    break;
            }

            if (!IsOnBoard(newPosition) || IsAnyAgentOn(newPosition))
                throw new InvalidMoveException();

            PlayerStates[playerId] = PlayerStates[playerId].ReconstructWithPosition(newPosition.X, newPosition.Y);
        }

        private bool IsOnBoard((int, int) newPosition)
        {
            (int x, int y) = newPosition;
            return x < Board.Height && x >= 0 &&
                y < Board.Width && y >= 0;
        }

        private bool IsAnyAgentOn((int X, int Y) newPosition)
        {
            foreach ((var id, var playerState) in PlayerStates)
            {
                if (playerState.Position == newPosition)
                {
                    return true;
                }
            }
            return false;
        }

        public void PickUpPiece(int playerId)
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            (int x, int y) = player.Position;

            if (!Board[x, y].HasPiece)
                throw new PieceOperationException("No piece on this field!");

            foreach(var playerState in PlayerStates)
            {
                if (playerState.Value.Piece != null)
                    throw new PieceOperationException("Another player already has a piece!");
            }
            Piece piece = Board[x, y].Piece;
            player.Piece = piece;
            Board.PiecesPositions.Remove((x,y));
            Board.Board[x, y].Piece = null;

            Board.RecalculateDistances();
        }

        private void PutPiece()
        {

        }
        private void DestroyPiece()
        {

        }
        private void DiscoverField()
        {

        }
        
        public bool IsStateCorrect()
        {
            return !Board.AreAnyPiecesInGoalArea() && Board.PieceCount <= maxPiecesOnBoard;
        }
    }

    public class DelayException: Exception { }
}
