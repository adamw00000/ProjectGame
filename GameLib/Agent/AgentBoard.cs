using System;

namespace GameLib
{
    public class AgentBoard
    {
        public readonly AgentField[,] BoardTable;
        public int Height => BoardTable.GetLength(1);
        public int Width => BoardTable.GetLength(0); //changed
        public int GoalAreaHeight { get; }

        public AgentBoard(AgentGameRules rules)
        {
            BoardTable = new AgentField[rules.BoardWidth, rules.BoardHeight];
            GoalAreaHeight = rules.GoalAreaHeight;

            void FillBoardRow(int y, bool isGoalArea)
            {
                for (int x = 0; x < Width; x++)
                {
                    BoardTable[x, y] = new AgentField()
                    {
                        IsGoal = isGoalArea ? AgentFieldState.Unknown : AgentFieldState.NA
                    };
                    BoardTable[x, y].SetDistance(-1, -1);
                }
            }

            for (int y = 0; y < GoalAreaHeight; y++)
            {
                FillBoardRow(y, true);
            }

            for (int y = GoalAreaHeight; y < Height - GoalAreaHeight; y++)
            {
                FillBoardRow(y, false);
            }

            for (int y = Height - GoalAreaHeight; y < Height; y++)
            {
                FillBoardRow(y, true);
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

        public void SetDistance(int x, int y, int distance, int timestamp)
        {
            BoardTable[x, y].SetDistance(distance, timestamp);
        }

        public void SetFieldState(int x, int y, AgentFieldState isGoal)
        {
            BoardTable[x, y].IsGoal = isGoal;
        }

        public void ApplyDiscoveryResult(DiscoveryResult discoveryResult, int timestamp)
        {
            foreach (var (x, y, distance) in discoveryResult.Fields)
            {
                if (!IsInBounds(x, y))
                    throw new InvalidDiscoveryResultException($"({x},{y}) is outside the board");
                BoardTable[x, y].SetDistance(distance, timestamp);
            }
        }

        public void ApplyCommunicationResult(AgentBoard partnerBoard)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (BoardTable[x, y].IsGoal == AgentFieldState.Unknown)
                        BoardTable[x, y].IsGoal = partnerBoard[x, y].IsGoal;

                    if (IsResultBoardOlder(partnerBoard, x, y))
                        continue;

                    BoardTable[x, y].SetDistance(partnerBoard[x, y].Distance, partnerBoard[x, y].Timestamp);
                }
            }
        }

        private bool IsResultBoardOlder(AgentBoard resultBoard, int x, int y)
        {
            return BoardTable[x, y].Timestamp >= resultBoard[x, y].Timestamp;
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }
    }
}