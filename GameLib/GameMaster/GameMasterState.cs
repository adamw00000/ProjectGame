using System;
using System.Collections.Generic;

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

        public void InitializePlayerPositions(int width, int height, int teamSize) //todo
        {
            for (int i = 0; i < teamSize; i++)
            {
                PlayerStates.Add(i, new PlayerState(i / width, width / 2 + Ceiling(i % width) * Power(i), Team.Blue, i == 0));
                PlayerStates.Add(i + teamSize, new PlayerState(height - 1 - i / width, width / 2 + Ceiling(i % width) * Power(i + 1), Team.Red, i == 0));
            }
            int Ceiling(int n)
            {
                return n % 2 == 0 ? n / 2 : n / 2 + 1;
            }
            int Power(int n)
            {
                return n % 2 == 0 ? 1 : -1;
            }
        }

        public Dictionary<int, GameRules> GetAgentGameRules()
        {
            Dictionary<int, GameRules> rules = new Dictionary<int, GameRules>();

            foreach (var (id, playerState) in PlayerStates)
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

        public void Move(int playerId, MoveDirection direction)
        {
            var player = PlayerStates[playerId];
            if (!player.IsEligibleForAction)
                throw new DelayException();

            var newPosition = player.Position;

            switch (direction)
            {
                case MoveDirection.Left:
                    newPosition.Y--;
                    break;

                case MoveDirection.Right:
                    newPosition.Y++;
                    break;

                case MoveDirection.Up:
                    newPosition.X--;
                    break;

                case MoveDirection.Down:
                    newPosition.X++;
                    break;
            }

            if (!IsOnBoard(newPosition) || IsAnyAgentOn(newPosition))
                throw new InvalidMoveException();

            var enemyTeam = player.Team == Team.Blue ? Team.Red : Team.Blue;
            if (Board.IsAgentInGoalArea(newPosition.X, newPosition.Y, enemyTeam))
                throw new InvalidMoveException();

            player.Position = newPosition;
            PlayerStates[playerId] = player;
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
            Board.PiecesPositions.Remove((x, y));
            Board.BoardTable[x, y].Piece = null;

            Board.RecalculateDistances();
            PlayerStates[playerId] = player;
        }

        public bool? PutPiece(int playerId) //PutPieceResult zamiast bool? ?
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            if (player.Piece == null)
                throw new PieceOperationException("Player doesn't have a piece!");

            (int x, int y) = player.Position;

            if (Board[x, y].HasPiece)
                throw new PieceOperationException("Cannot put another piece on this field!");

            if (Board.IsAgentInGoalArea(x, y, player.Team))
            {
                return PutPieceInGoalArea(playerId, player, x, y);
            }
            else
            {
                PutPieceInTaskArea(playerId, player, x, y);
                return null;
            }
        }

        private bool? PutPieceInGoalArea(int playerId, PlayerState player, int x, int y)
        {
            bool? result = null;
            if (player.Piece.IsValid)
            {
                result = Board[x, y].IsGoal;
            }
            DestroyPlayersPiece(playerId, player);

            return result;
        }

        private void DestroyPlayersPiece(int playerId, PlayerState player)
        {
            player.Piece = null;
            PlayerStates[playerId] = player;
            Board.PieceCount--;
        }

        private void PutPieceInTaskArea(int playerId, PlayerState player, int x, int y)
        {
            Board.BoardTable[x, y].Piece = PlayerStates[playerId].Piece;
            Board.PiecesPositions.Add((x, y));
            player.Piece = null;
            PlayerStates[playerId] = player;

            Board.RecalculateDistances();
        }

        public void DestroyPiece(int playerId)
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            if (player.Piece == null)
                throw new PieceOperationException("Player doesn't have a piece!");

            DestroyPlayersPiece(playerId, player);
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

        public (int[,], (int x, int y)) DiscoverField(int playerId) //to be discussed
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            return (Board.GetDistancesAround(player.Position.X, player.Position.Y), (player.Position.X, player.Position.Y));
        }
    }
}