﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLib
{
    public class GameMasterState
    {
        public readonly bool GameEnded = false;
        private readonly double validPieceProbability;
        private readonly int maxPiecesOnBoard;
        private readonly GameRules gameRules;
        private readonly Dictionary<(int senderId, int targetId), object> CommunicationData = new Dictionary<(int senderId, int targetId), object>();

        public readonly GameMasterBoard Board;
        public Dictionary<int, PlayerState> PlayerStates = new Dictionary<int, PlayerState>();

        public GameMasterState(GameRules rules)
        {
            gameRules = rules;
            Board = new GameMasterBoard(gameRules);
            validPieceProbability = 1 - rules.BadPieceProbability;
            maxPiecesOnBoard = rules.MaxPiecesOnBoard;
        }

        public void InitializePlayerPositions(int width, int height, int teamSize)
        {
            List<PlayerState> reds = new List<PlayerState>(PlayerStates.Values.Where(player => player.Team == Team.Red));
            List<PlayerState> blues = new List<PlayerState>(PlayerStates.Values.Where(player => player.Team == Team.Blue));
            for (int i = 0; i < teamSize; i++)
            {
                /*PlayerStates.Add(i, new PlayerState(i / width, width / 2 + HalfCeiling(i % width) * Side(i), Team.Blue, i == 0));
                PlayerStates.Add(i + teamSize, new PlayerState(height - 1 - i / width, width / 2 + HalfCeiling(i % width) * Side(i), Team.Red, i == 0));*/
                PlayerState redPlayer = reds[i];
                PlayerState bluePlayer = blues[i];
                redPlayer.Position = (i / width, width / 2 + HalfCeiling(i % width) * Side(i));
                bluePlayer.Position = (height - 1 - i / width, width / 2 + HalfCeiling(i % width) * Side(i));
            }
            int HalfCeiling(int n)
            {
                return n % 2 == 0 ? n / 2 : n / 2 + 1;
            }
            int Side(int n)
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

        public void DelayPlayer(int playerId, int delayMultiplier)
        {
            var player = PlayerStates[playerId];
            player.LastRequestTimestamp = DateTime.UtcNow;
            player.LastActionDelay = delayMultiplier * gameRules.BaseTimePenalty;
            PlayerStates[playerId] = player;
        }

        public void GeneratePiece()
        {
            if (Board.PieceCount < maxPiecesOnBoard)
                Board.GeneratePiece(validPieceProbability);
        }

        public void GeneratePieceAt(int x, int y)
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

            if (!IsOnBoard(newPosition))
                throw new InvalidMoveException("Agent tried to move out of board!");

            if (IsAnyAgentOn(newPosition))
                throw new InvalidMoveException("Agent tried to on the space occupied by another agent!");

            var enemyTeam = player.Team == Team.Blue ? Team.Red : Team.Blue;
            if (Board.IsAgentInGoalArea(newPosition.X, newPosition.Y, enemyTeam))
                throw new InvalidMoveException("Agent tried to move onto enemy goal area!");

            player.Position = newPosition;
            PlayerStates[playerId] = player;

            DelayPlayer(playerId, gameRules.MoveMultiplier);
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

            DelayPlayer(playerId, gameRules.PickUpMultiplier);
        }

        public PutPieceResult PutPiece(int playerId) //PutPieceResult zamiast bool? ?
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            if (player.Piece == null)
                throw new PieceOperationException("Player doesn't have a piece!");

            (int x, int y) = player.Position;

            if (Board[x, y].HasPiece)
                throw new PieceOperationException("Cannot put another piece on this field!");

            DelayPlayer(playerId, gameRules.PutPieceMultiplier);

            if (Board.IsAgentInGoalArea(x, y, player.Team))
            {
                return PutPieceInGoalArea(playerId, x, y);
            }
            else
            {
                PutPieceInTaskArea(playerId, x, y);
                return PutPieceResult.PieceInTaskArea;
            }
        }

        private PutPieceResult PutPieceInGoalArea(int playerId, int x, int y)
        {
            PlayerState player = PlayerStates[playerId];
            PutPieceResult result = PutPieceResult.PieceWasFake;
            if (player.Piece.IsValid)
            {
                result = Board[x, y].IsGoal ? PutPieceResult.PieceGoalRealized : PutPieceResult.PieceGoalUnrealized;
            }
            DestroyPlayersPiece(playerId);
            return result;
        }

        private void PutPieceInTaskArea(int playerId, int x, int y)
        {
            PlayerState player = PlayerStates[playerId];
            Board.BoardTable[x, y].Piece = PlayerStates[playerId].Piece;
            Board.PiecesPositions.Add((x, y));
            player.Piece = null;
            PlayerStates[playerId] = player;

            Board.RecalculateDistances();
        }

        private void DestroyPlayersPiece(int playerId)
        {
            PlayerState player = PlayerStates[playerId];
            player.Piece = null;
            PlayerStates[playerId] = player;
            Board.PieceCount--;
        }

        public void DestroyPiece(int playerId)
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            if (player.Piece == null)
                throw new PieceOperationException("Player doesn't have a piece!");

            DestroyPlayersPiece(playerId);

            DelayPlayer(playerId, gameRules.DestroyPieceMultiplier);
        }

        public bool CheckPiece(int playerId)
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            if (player.Piece == null)
                throw new PieceOperationException("Player doesn't have a piece!");

            DelayPlayer(playerId, gameRules.CheckMultiplier);

            return player.Piece.IsValid;
        }

        public int[,] Discover(int playerId)
        {
            PlayerState player = PlayerStates[playerId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            DelayPlayer(playerId, gameRules.DiscoverMultiplier);

            return Board.GetDistancesAround(player.Position.X, player.Position.Y);
        }

        //Communication scheme:
        //agent -> gm (gm saves data to the dictionary) //SaveCommunicationData
        //gm -> ag2 (generic message, doesn't depend on gm state)
        //ag2 -> gm (DelayException if target is delayed, adds delay to the sender (doesn't check)) //(GetCommunicationData to check if the communication exists) + DelayCommunicationPartners
        //gm -> ag2 (gm reads data from the dictionary) //sends data obtained from GetCommunicationData
        //gm -> ag1 (forwarding data, doesn't depend on gm state)

        public void DelayCommunicationPartners(int senderId, int targetId)
        {
            PlayerState player = PlayerStates[targetId];

            AddDelay(targetId, gameRules.CommunicationMultiplier);
            AddDelay(senderId, gameRules.CommunicationMultiplier);
        }

        private void AddDelay(int playerId, int delayMultiplier)
        {
            var player = PlayerStates[playerId];
            int lastPenaltyDurationLeftTime = Math.Max(0, player.LastActionDelay - (int)(DateTime.UtcNow - player.LastRequestTimestamp).TotalMilliseconds);
            player.LastActionDelay = delayMultiplier * gameRules.BaseTimePenalty + lastPenaltyDurationLeftTime;
            player.LastRequestTimestamp = DateTime.UtcNow;
            PlayerStates[playerId] = player;
        }

        public void SaveCommunicationData(int senderId, int targetId, object data)
        {
            PlayerState player = PlayerStates[targetId];

            if (!player.IsEligibleForAction)
                throw new DelayException();

            CommunicationData[(senderId, targetId)] = data;
        }

        public object GetCommunicationData(int senderId, int targetId)
        {
            if (CommunicationData.TryGetValue((senderId, targetId), out object data))
            {
                return data;
            }
            else
            {
                throw new CommunicationException($"Communication data for pair ({senderId}, {targetId}) doesn't exist!");
            }
        }

        public void JoinGame(int agentId, int teamId, bool wantToBeLeader)
        {
            if (PlayerStates.ContainsKey(agentId))
                throw new GameSetupException($"Agent with Id {agentId} is already connected.");

            if (teamId != 0 && teamId != 1)
                throw new GameSetupException($"No team with Id {teamId}");

            Team team = (Team)teamId;

            int teamMembers = PlayerStates.Count(p => p.Value.Team == team);

            if (teamMembers >= gameRules.TeamSize)
                throw new GameSetupException($"Team ${teamId} is full");

            bool isLeaderInTeam = PlayerStates.Any(p => p.Value.IsLeader);

            bool willBeLeader;
            if(isLeaderInTeam)
            {
                willBeLeader = false;
            }
            else if (teamMembers == gameRules.TeamSize - 1)
            {
                willBeLeader = true;
            }
            else
            {
                willBeLeader = wantToBeLeader;
            }

            PlayerStates.Add(agentId, new PlayerState(-1, -1, team, willBeLeader));
        }

        public void Co(int senderId, int targetId, object data)
        {
            CommunicationData[(senderId, targetId)] = data;
        }
}
}