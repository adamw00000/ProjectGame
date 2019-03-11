using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GameLib.Exceptions;

namespace GameLib
{
    public class GameMasterState
    {
        public readonly bool GameEnded = false;
        private readonly double validPieceProbability;
        private readonly int maxPiecesOnBoard;
        private readonly GameRules gameRules;

        public readonly GameMasterBoard Board;
        public Dictionary<int, PlayerState> PlayerStates = new Dictionary<int, PlayerState>();

        public GameMasterState(GameRules rules)
        {
            gameRules = rules;
            Board = new GameMasterBoard(gameRules);
            validPieceProbability = 1 - rules.BadPieceProbability;
            maxPiecesOnBoard = rules.MaxPiecesOnBoard;
        }

        public void AddPlayer(int id, Team team, bool isLeader) //todo
        {
            throw new NotImplementedException();
        }

        public void InitializePlayerPositions(int width, int height, int goalAreaHeight, int teamSize) //todo
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, GameRules> GetAgentGameRules()
        {
            Dictionary<int, GameRules> rules = new Dictionary<int, GameRules>();

            foreach(var (id, playerState) in PlayerStates)
            {
                var privateRules = gameRules.ReconstructWithAgentPosition(playerState.Position.X, playerState.Position.Y);
                rules.Add(id, privateRules);
            }

            return rules;
        }

        public void GeneratePiece()
        {
            if (Board.PieceCount < maxPiecesOnBoard)
                Board.GeneratePiece(validPieceProbability);
        }
        public void GeneratePieceAt(int x, int y) //do testow!
        {
            if (Board.PieceCount < maxPiecesOnBoard)
                Board.GeneratePieceAt(x, y, validPieceProbability);
        }

        public void Move(int playerId, Direction direction)
        {
            if (!PlayerStates[playerId].IsEligibleForAction)
                throw new DelayException();

            var newPosition = PlayerStates[playerId].Position;

            switch (direction)
            {
                case Direction.Left:
                    newPosition.Y--;
                    break;
                case Direction.Right:
                    newPosition.Y++;
                    break;
                case Direction.Up:
                    newPosition.X--;
                    break;
                case Direction.Down:
                    newPosition.X++;
                    break;
            }

            if (!IsOnBoard(newPosition) || IsAnyAgentOn(newPosition)) 
                throw new InvalidMoveException();

            var enemyTeam = PlayerStates[playerId].Team == Team.Blue ? Team.Red : Team.Blue;
            if (Board.IsAgentInGoalArea(newPosition.X, newPosition.Y, enemyTeam))
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

            player.Piece = Board[x, y].Piece;
            Board.PiecesPositions.Remove((x,y));
            Board.BoardTable[x, y].Piece = null;

            Board.RecalculateDistances();
        }

        public bool PutPiece(int playerId)
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            if(player.Piece == null)
                throw new PieceOperationException("Player doesn't have a piece!");

            (int x, int y) = player.Position;

            if (Board[x, y].HasPiece)
                throw new PieceOperationException("Cannot put another piece on this field!");

            if (Board.IsAgentInGoalArea(x, y, player.Team))
            {
                return PutPieceInGoalArea(player, x, y);
            }
            else
            {
                PutPieceInTaskArea(player, x, y);
                return false;
            }
        }

        private bool PutPieceInGoalArea(PlayerState player, int x, int y)
        {
            bool result;
            if (player.Piece.IsValid && Board[x, y].IsGoal)
            {
                result = true;
            }

            DestroyPlayersPiece(player);

            result = false;
            return result;
        }

        private void DestroyPlayersPiece(PlayerState player)
        {
            player.Piece = null;
            Board.PieceCount--;
        }

        private void PutPieceInTaskArea(PlayerState player, int x, int y)
        {
            Board.BoardTable[x, y].Piece = player.Piece;
            Board.PiecesPositions.Add((x, y));
            player.Piece = null;

            Board.RecalculateDistances();
        }

        public void DestroyPiece(int playerId)
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            if (player.Piece == null)
                throw new PieceOperationException("Player doesn't have a piece!");

            DestroyPlayersPiece(player);
        }

        public (int[,],(int x, int y)) DiscoverField(int playerId)
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            return (Board.GetDistancesAround(player.Position.X, player.Position.Y), (player.Position.X, player.Position.Y));
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
    }

}
