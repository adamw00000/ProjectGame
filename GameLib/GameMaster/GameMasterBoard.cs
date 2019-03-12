using System.Collections.Generic;

namespace GameLib
{
    public class GameMasterBoard
    {
        public readonly GameMasterField[,] BoardTable;
        public int Height => BoardTable.GetLength(0);
        public int Width => BoardTable.GetLength(1);
        public int GoalAreaHeight { get; }

        public int PieceCount { get; set; } = 0; //wlacznie z kawalkami posiadanymi przez graczy
        public List<(int x, int y)> PiecesPositions = new List<(int x, int y)>();

        public GameMasterBoard(GameRules rules)
        {
            int width = rules.BoardWidth;
            int height = rules.BoardHeight;
            BoardTable = new GameMasterField[height, width];

            GoalAreaHeight = rules.GoalAreaHeight;

            GenerateGoals(rules.GoalCount);

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    BoardTable[x, y] = new GameMasterField() { Distance = int.MaxValue }; //Niektore pola z goal area sa golami, trzeba to uswawiac
                }
            }
        }

        private void GenerateGoals(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ChooseRandomFieldForGoal(out int x, out int y);
                BoardTable[x, y].IsGoal = true;
            }
        }

        private void ChooseRandomFieldForGoal(out int x, out int y)
        {
            var random = RandomGenerator.GetGenerator();

            do
            {
                x = random.Next(GoalAreaHeight, Height - GoalAreaHeight);
                y = random.Next(0, Width);
            } while (BoardTable[x, y].IsGoal); //Madrzejsze losowanie? da sie o(1), todo
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
                return x >= 0 && x < GoalAreaHeight;
            }
            else
            {
                return x >= Height - GoalAreaHeight && x < Height;
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
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
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
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    BoardTable[x, y].Distance = -1;
                }
            }
        }

        private void CalculateDistancesFromPiece(int x, int y, int distance) //a la flood fill
        {
            bool[,] visited = new bool[Width, Height];

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

                        if (px + i <= Width - 1 && px + i >= 0 && !visited[px + i, py])
                        {
                            stack.Push((px + i, py, dist + 1));
                        }

                        if (py + j <= Height - 1 && py + j >= 0 && !visited[px, py + j])
                        {
                            stack.Push((px, py + j, dist + 1));
                        }

                        if (py + j <= Height - 1 && py + j >= 0 && px + i <= Width - 1 && px + i >= 0 && !visited[px + i, py + j])
                        {
                            stack.Push((px + i, py + j, dist + 2));
                        }
                    }
                }
            }
        }

        public void GeneratePiece(double probability)
        {
            ChooseRandomFieldForPiece(out int x, out int y);

            BoardTable[x, y].Piece = new Piece(probability);

            PiecesPositions.Add((x, y));
            PieceCount++;
            RecalculateDistances();
        }

        public void GeneratePieceAt(int x, int y, double probability) //do testow!!
        {
            BoardTable[x, y].Piece = new Piece(probability);

            PiecesPositions.Add((x, y));
            PieceCount++;
            RecalculateDistances();
        }

        private void ChooseRandomFieldForPiece(out int x, out int y)
        {
            var random = RandomGenerator.GetGenerator();

            do
            {
                x = random.Next(GoalAreaHeight, Height - GoalAreaHeight);
                y = random.Next(0, Width);
            } while (BoardTable[x, y].HasPiece); //Madrzejsze losowanie? da sie o(1), todo
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