using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class AgentBoard
    {
        public readonly AgentField[,] Board;
        public int Height => Board.GetLength(0);
        public int Width => Board.GetLength(1);
        public int GoalAreaHeight { get; }

        public AgentBoard(GameRules rules)
        {
            // konstrukcja tablicy i wypełnienie AgentField.FieldState odpowiednimi wartościami
            Board = new AgentField[rules.BoardHeight, rules.BoardWidth];
            GoalAreaHeight = rules.GoalAreaHeight;

            void FillBoardRow(int i, bool isGoalArea)
            {
                for (int j = 0; j < Width; j++)
                {
                    Board[i, j] = new AgentField() { Distance = int.MaxValue, Timestamp = DateTime.UtcNow,
                        IsGoal = isGoalArea ? FieldState.Unknown : FieldState.NA };
                }
            }

            for (int i = 0; i < GoalAreaHeight; i++)
            {
                FillBoardRow(i, true);
            }

            for (int i = GoalAreaHeight; i < Height - GoalAreaHeight; i++)
            {
                FillBoardRow(i, false);
            }

            for (int i = Height - GoalAreaHeight; i < Height; i++)
            {
                FillBoardRow(i, true);
            }
        }

        public AgentField this[int x, int y]
        {
            get => Board[x, y];
            set
            {
                Board[x, y] = value;
            }
        }
        
        public void SetDistance(int x, int y, int distance)
        {
            Board[x, y].Distance = distance;
        }

        public void SetFieldState(int x, int y, FieldState isGoal)
        {
            Board[x, y].IsGoal = isGoal;
        }

        internal void ApplyDiscoveryResult(DiscoveryResult discoveryResult)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!IsInBounds(discoveryResult, i, j))
                        continue;
                    
                    Board[discoveryResult.BasePosition.X + i, discoveryResult.BasePosition.Y + j].Distance =
                        discoveryResult.Fields[i + 1, j + 1].Distance;
                }
            }
        }

        private bool IsInBounds(DiscoveryResult discoveryResult, int i, int j)
        {
            return discoveryResult.BasePosition.X + i >= 0 &&
                discoveryResult.BasePosition.X + i < Width &&
                discoveryResult.BasePosition.Y + j >= 0 &&
                discoveryResult.BasePosition.Y + j < Height;
        }

        internal void ApplyCommunicationResult(AgentBoard resultBoard)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (IsResultBoardOlder(resultBoard, i, j))
                        continue;

                    Board[i, j].Distance = resultBoard[i, j].Distance;
                    Board[i, j].IsGoal = resultBoard[i, j].IsGoal;
                    Board[i, j].Timestamp = resultBoard[i, j].Timestamp;
                }
            }
        }

        private bool IsResultBoardOlder(AgentBoard resultBoard, int i, int j)
        {
            return Board[i, j].Timestamp >= resultBoard[i, j].Timestamp;
        }
    }
}