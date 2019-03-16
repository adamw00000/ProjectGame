using System.Collections.Generic;

namespace GameLib
{
    public class GameMasterBoard
    {
        public readonly GameMasterField[,] BoardTable;
        public int Height => BoardTable.GetLength(0);
        public int Width => BoardTable.GetLength(1);
        public int GoalAreaHeight { get; }

        public int PieceCount { get; set; } = 0; //including ones possessed by agents
        public List<(int x, int y)> PiecesPositions = new List<(int x, int y)>();

        public GameMasterBoard(GameRules rules)
        {
            int width = rules.BoardWidth;
            int height = rules.BoardHeight;
            BoardTable = new GameMasterField[height, width];

            GoalAreaHeight = rules.GoalAreaHeight;

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    BoardTable[x, y] = new GameMasterField() { Distance = int.MaxValue };
                }
            }

            GenerateGoals(rules.GoalCount);
        }

        private void GenerateGoals(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ChooseRandomFieldForGoal(out int x, out int y, Team.Red);
                BoardTable[x, y].IsGoal = true;
            }
            for (int i = 0; i < count; i++)
            {
                ChooseRandomFieldForGoal(out int x, out int y, Team.Blue);
                BoardTable[x, y].IsGoal = true;
            }
        }

        private void ChooseRandomFieldForGoal(out int x, out int y, Team team)
        {
            var random = RandomGenerator.GetGenerator();

            do
            {
                if (team == Team.Red)
                {
                    x = random.Next(0, GoalAreaHeight);
                }
                else
                {
                    x = random.Next(Height - GoalAreaHeight, Height);
                }
                y = random.Next(0, Width);
            } while (BoardTable[x, y].IsGoal); //o(1) implementation is possible
        }

        public GameMasterField this[int x, int y]
        {
            get => BoardTable[x, y];
            set
            {
                BoardTable[x, y] = value;
            }
        }

        public bool IsAgentInGoalArea(int x, int _, Team team)
        {
            if (team == Team.Blue)
            {
                return x >= Height - GoalAreaHeight && x < Height;
            }
            else
            {
                return x >= 0 && x < GoalAreaHeight;
            }
        }

        public void RecalculateDistances()
        {
            if (PiecesPositions.Count == 0)
            {
                SetEmptyBoardDistances();
            }
            else
            {
                for (int x = 0; x < Height; x++)
                {
                    for (int y = 0; y < Width; y++)
                    {
                        BoardTable[x, y].Distance = int.MaxValue;
                    }
                }

                foreach (var (x, y) in PiecesPositions)
                {
                    CalculateDistancesFromPiece(x, y, 0);
                }
            }
        }

        private void SetEmptyBoardDistances()
        {
            for (int x = 0; x < Height; x++)
            {
                for (int y = 0; y < Width; y++)
                {
                    BoardTable[x, y].Distance = -1;
                }
            }
        }

        private void CalculateDistancesFromPiece(int x, int y, int distance) //a la flood fill
        {
            bool[,] visited = new bool[Height, Width];

            Stack<(int px, int py, int dist)> stack = new Stack<(int x, int y, int dist)>();

            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    stack.Push((x, y, distance));
                    while (stack.Count > 0)
                    {
                        (int px, int py, int dist) = stack.Pop();
                        visited[px, py] = true;

                        if (BoardTable[px, py].Distance > dist)
                        {
                            BoardTable[px, py].Distance = dist;
                        }

                        if (px + i <= Height - 1 && px + i >= 0 && !visited[px + i, py])
                        {
                            stack.Push((px + i, py, dist + 1));
                        }

                        if (py + j <= Width - 1 && py + j >= 0 && !visited[px, py + j])
                        {
                            stack.Push((px, py + j, dist + 1));
                        }

                        if (py + j <= Width - 1 && py + j >= 0 && px + i <= Height - 1 && px + i >= 0 && !visited[px + i, py + j])
                        {
                            stack.Push((px + i, py + j, dist + 2));
                        }
                    }
                }
            }
        }

        public void GeneratePiece(double probability)
        {
            (int x, int y) = ChooseRandomFieldForPiece();

            BoardTable[x, y].Piece = new Piece(probability);

            PiecesPositions.Add((x, y));
            PieceCount++;
            RecalculateDistances();
        }

        public void GeneratePieceAt(int x, int y, double probability)
        {
            if (BoardTable[x, y].HasPiece)
                throw new PieceOperationException($"Cannot generate piece on the field ({x},{y}) - this field already has piece.");

            BoardTable[x, y].Piece = new Piece(probability);

            PiecesPositions.Add((x, y));
            PieceCount++;
            RecalculateDistances();
        }

        private (int x, int y) ChooseRandomFieldForPiece()
        {
            var random = RandomGenerator.GetGenerator();
            int x;
            int y;

            do
            {
                x = random.Next(GoalAreaHeight, Height - GoalAreaHeight);
                y = random.Next(0, Width);
            } while (BoardTable[x, y].HasPiece); //o(1) implementation possible

            return (x, y);
        }

        public int[,] GetDistancesAround(int x, int y)
        {
            int[,] result = new int[3, 3];

            for (int i = x - 1; i < x + 2; i++)
            {
                for (int j = y - 1; j < y + 2; j++)
                {
                    if (i >= 0 && i < Width && j >= 0 && j < Height)
                    {
                        result[i - x + 1, j - y + 1] = BoardTable[i, j].Distance;
                    }
                    else
                    {
                        result[i - x + 1, j - y + 1] = int.MaxValue;
                    }
                }
            }
            return result;
        }

        public bool AreAnyPiecesInGoalArea()
        {
            for (int i = 0; i < GoalAreaHeight; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (BoardTable[i, j].Piece != null)
                        return true;
                }
            }

            for (int i = Height - GoalAreaHeight; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (BoardTable[i, j].Piece != null)
                        return true;
                }
            }

            return false;
        }
    }
}