using System.Collections.Generic;

namespace GameLib
{
    public class GameMasterBoard
    {
        public readonly GameMasterField[,] Board;
        public int Height => Board.GetLength(0);
        public int Width => Board.GetLength(1);
        public int GoalAreaHeight { get; }

        public int PieceCount { get; set; } = 0; //wlacznie z kawalkami posiadanymi przez graczy
        public List<(int x, int y)> PiecesPositions = new List<(int x, int y)>();

        public GameMasterBoard(GameRules rules)
        {
            int width = rules.BoardWidth;
            int height = rules.BoardHeight;
            Board = new GameMasterField[height, width];

            GoalAreaHeight = rules.GoalAreaHeight;

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    Board[x, y] = new GameMasterField() { Distance = int.MaxValue}; //Niektore pola z goal area sa golami, trzeba to uswawiac
                }
            }
        }
        public bool InHisGoalArea(int x, int y, int team)
        {
            if (team == 0)
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
            if (PiecesPositions.Count == 0) //PieceCount==0?? Chyba nie...?
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Board[x, y].Distance = -1;
                    }
                }
            }
            else
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Board[x, y].Distance = int.MaxValue;
                    }
                }
                bool[,] visited = new bool[Width, Height];
                foreach (var position in PiecesPositions)
                {
                    CalculateDistancesFromPiece(position.x, position.y, 0);
                }
            }
        }
        private void CalculateDistancesFromPiece(int x, int y, int distance)
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

                        if (Board[px, py].Distance > dist)
                        {
                            Board[px, py].Distance = dist;
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
            var random = RandomGenerator.GetGenerator();
            
            ChooseRandomField(random, out int x, out int y);

            Board[x, y].Piece = new Piece(probability);

            PiecesPositions.Add((x, y));
            PieceCount++;
            RecalculateDistances();
        }
        public void GeneratePieceOnCoordinates(int x, int y, double probability) //do testow!!
        {
            Board[x, y].Piece = new Piece(probability);

            PiecesPositions.Add((x, y));
            PieceCount++;
            RecalculateDistances();
        }

        private void ChooseRandomField(System.Random random, out int x, out int y)
        {
            do
            {
                x = random.Next(GoalAreaHeight, Height - GoalAreaHeight);
                y = random.Next(0, Width);
            } while (Board[x, y].HasPiece); //Madrzejsze losowanie?
        }

        public int[,] GetDistancesAround(int x, int y)
        {
            int[,] result = new int[3, 3];

            for (int i = x - 1; i < x + 1; i++)
            {
                for (int j = y - 1; j < y + 1; j++)
                {
                    if (i >= 0 && i < Width && j >= 0 && j < Height)
                    {
                        result[i - x + 1, j - y + 1] = Board[i, j].Distance;
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
                    if (Board[i, j].Piece != null)
                        return true;
                }
            }

            for (int i = Height - GoalAreaHeight; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (Board[i, j].Piece != null)
                        return true;
                }
            }

            return false;
        }
        public GameMasterField this[int x, int y]
        {
            get => Board[x, y];
            set
            {
                Board[x, y] = value;
            }
        }
    }
}