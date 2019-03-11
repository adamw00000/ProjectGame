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
            //Trzeba zainicjowac PlayerStates
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

            //Jeszcze jeden if - nie mozna wchodzic do GoalArea przeciwnika

            PlayerStates[playerId] = PlayerStates[playerId].ReconstructWithPosition(newPosition.X, newPosition.Y);
        }

        public void PickUpPiece(int playerId)
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            (int x, int y) = player.Position;

            if (!Board[x, y].HasPiece)
                throw new PieceOperationException("No piece on this field!");

            foreach(var playerState in PlayerStates) //Czy moze być podniesiony wiecej niz jeden kawalek na raz? Przez 1 druzyne, obie?
            {
                if (playerState.Value.Piece != null)
                    throw new PieceOperationException("Another player already has a piece!");
            }

            player.Piece = Board[x, y].Piece; ;
            Board.PiecesPositions.Remove((x,y));
            Board.Board[x, y].Piece = null;

            Board.RecalculateDistances();
        }

        public void PutPiece(int playerId)
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            if(player.Piece == null)
                throw new PieceOperationException("Player doesn't have a piece!");

            (int x, int y) = player.Position;

            if (Board[x, y].HasPiece)
                throw new PieceOperationException("Cannot put another piece on this field!");

            if (Board.InHisGoalArea(x, y, player.Team))
            {
                if (player.Piece.IsValid)
                {
                    if (Board[x, y].IsGoal) //Jezeli pole jest goalem
                    {
                        //Poinformuj gracza o odkryciu goala

                        //if(to ostatni goal danej druzyny) GameEnded = true;
                    }
                    else
                    {
                        //Poinfoirmuj gracza o tym, ze to nie byl goal
                    }
                }
                player.Piece = null;
                Board.PieceCount--;
            }
            else
            {
                Board.Board[x, y].Piece = player.Piece;
                Board.PiecesPositions.Add((x, y));
                player.Piece = null;

                Board.RecalculateDistances();
            }
        }
        public void DestroyPiece(int playerId)
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            if (player.Piece == null)
                throw new PieceOperationException("Player doesn't have a piece!");

            player.Piece = null;
            Board.PieceCount--;
        }
        public int[,] DiscoverField(int playerId)
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            return Board.GetDistancesAround(player.Position.X, player.Position.Y);
        }

        public bool CheckPiece(int playerId)
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            if (player.Piece == null)
                throw new PieceOperationException("Player doesn't have a piece!");

            return player.Piece.IsValid;

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
        public bool IsStateCorrect()
        {
            return !Board.AreAnyPiecesInGoalArea() && Board.PieceCount <= maxPiecesOnBoard;
        }
    }

    public class DelayException: Exception { }
}
