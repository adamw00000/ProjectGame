using System;

namespace GameLib
{
    public class AgentBoard
    {
        public readonly AgentField[,] BoardTable;
        public int Height => BoardTable.GetLength(0);
        public int Width => BoardTable.GetLength(1);
        public int GoalAreaHeight { get; }

        public AgentBoard(GameRules rules)
        {
            BoardTable = new AgentField[rules.BoardHeight, rules.BoardWidth];
            GoalAreaHeight = rules.GoalAreaHeight;

            void FillBoardRow(int i, bool isGoalArea)
            {
                for (int j = 0; j < Width; j++)
                {
                    BoardTable[i, j] = new AgentField()
                    {
                        Distance = -1,
                        Timestamp = DateTime.UtcNow,
                        IsGoal = isGoalArea ? FieldState.Unknown : FieldState.NA
                    };
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
            get => BoardTable[x, y];
            set
            {
                BoardTable[x, y] = value;
            }
        }

        public void SetDistance(int x, int y, int distance)
        {
            BoardTable[x, y].Distance = distance;
        }

        public void SetFieldState(int x, int y, FieldState isGoal)
        {
            BoardTable[x, y].IsGoal = isGoal;
        }

        internal void ApplyDiscoveryResult(AgentDiscoveryResult discoveryResult)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!IsInBounds(discoveryResult, i, j))
                        continue;

                    BoardTable[discoveryResult.BasePosition.X + i, discoveryResult.BasePosition.Y + j].Distance =
                        discoveryResult.Fields[i + 1, j + 1].Distance;
                }
            }
        }

        private bool IsInBounds(AgentDiscoveryResult discoveryResult, int i, int j)
        {
            return discoveryResult.BasePosition.X + i >= 0 &&
                discoveryResult.BasePosition.X + i < Width &&
                discoveryResult.BasePosition.Y + j >= 0 &&
                discoveryResult.BasePosition.Y + j < Height;
        }

        internal void UpdateBoardWithCommunicationData(AgentBoard partnerBoard)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (IsResultBoardOlder(partnerBoard, i, j))
                        continue;

                    BoardTable[i, j].Distance = partnerBoard[i, j].Distance;
                    BoardTable[i, j].IsGoal = partnerBoard[i, j].IsGoal;
                    BoardTable[i, j].Timestamp = partnerBoard[i, j].Timestamp;
                }
            }
        }

        private bool IsResultBoardOlder(AgentBoard resultBoard, int i, int j)
        {
            return BoardTable[i, j].Timestamp >= resultBoard[i, j].Timestamp;
        }
    }
}