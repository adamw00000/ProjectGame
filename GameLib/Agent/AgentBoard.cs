using System;

namespace GameLib
{
    public class AgentBoard
    {
        public readonly AgentField[,] BoardTable;
        public int Height => BoardTable.GetLength(0);
        public int Width => BoardTable.GetLength(1);
        public int GoalAreaHeight { get; }

        public AgentBoard(AgentGameRules rules)
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
                        IsGoal = isGoalArea ? AgentFieldState.Unknown : AgentFieldState.NA
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

        public void SetFieldState(int x, int y, AgentFieldState isGoal)
        {
            BoardTable[x, y].IsGoal = isGoal;
        }

        public void ApplyDiscoveryResult(DiscoveryResult discoveryResult, int timestamp)
        {
            foreach(var (x,y,distance) in discoveryResult.Fields)
            {
                if (!IsInBounds(x, y))
                    throw new InvalidDiscoveryResultException($"({x},{y}) is outside the board");
                BoardTable[x, y].Distance = distance;
                BoardTable[x, y].Timestamp = (new DateTime()).AddMilliseconds(timestamp);
            }
        }

        public void ApplyCommunicationResult(AgentBoard partnerBoard)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (BoardTable[i, j].IsGoal == AgentFieldState.Unknown)
                        BoardTable[i, j].IsGoal = partnerBoard[i, j].IsGoal;

                    if (IsResultBoardOlder(partnerBoard, i, j))
                        continue;

                    BoardTable[i, j].Distance = partnerBoard[i, j].Distance;
                    BoardTable[i, j].Timestamp = partnerBoard[i, j].Timestamp;
                }
            }
        }

        private bool IsResultBoardOlder(AgentBoard resultBoard, int i, int j)
        {
            return BoardTable[i, j].Timestamp >= resultBoard[i, j].Timestamp;
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Height && y < Width;
        }
    }
}