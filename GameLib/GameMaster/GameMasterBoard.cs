using System;
using System.Collections.Generic;

namespace GameLib
{
    public class GameMasterBoard: ICloneable
    {
        public readonly GameMasterField[,] BoardTable;
        public int Height => BoardTable.GetLength(1);
        public int Width => BoardTable.GetLength(0);
        public int GoalAreaHeight { get; }

        public int PieceCount { get; set; } = 0; //including ones possessed by agents
        public List<(int x, int y)> PiecesPositions = new List<(int x, int y)>();

        private GameMasterBoard(int height, int width, int goalAreaHeight)
        {
            this.GoalAreaHeight = goalAreaHeight;
            this.BoardTable = new GameMasterField[height, width];
        }

        public GameMasterBoard(GameRules rules)
        {
            int width = rules.BoardWidth;
            int height = rules.BoardHeight;
            BoardTable = new GameMasterField[width, height];

            GoalAreaHeight = rules.GoalAreaHeight;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
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
                if (team == Team.Blue)
                {
                    y = random.Next(0, GoalAreaHeight);
                }
                else
                {
                    y = random.Next(Height - GoalAreaHeight, Height);
                }
                x = random.Next(0, Width);
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

        public bool IsAgentInGoalArea(int x, int y, Team team)
        {
            if (team == Team.Red)
            {
                return y >= Height - GoalAreaHeight && y < Height;
            }
            else
            {
                return y >= 0 && y < GoalAreaHeight;
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

            Stack<(int px, int py, int distance)> stack = new Stack<(int x, int y, int distance)>();

            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    stack.Push((x, y, distance));
                    while (stack.Count > 0)
                    {
                        (int px, int py, int pdistance) = stack.Pop();
                        visited[px, py] = true;

                        if (BoardTable[px, py].Distance > pdistance)
                        {
                            BoardTable[px, py].Distance = pdistance;
                        }

                        if (py + i <= Height - 1 && py + i >= 0 && !visited[px, py + i])
                        {
                            stack.Push((px, py + i, pdistance + 1));
                        }

                        if (px + j <= Width - 1 && px + j >= 0 && !visited[px + j, py])
                        {
                            stack.Push((px + j, py, pdistance + 1));
                        }

                        if (px + j <= Width - 1 && px + j >= 0 && py + i <= Height - 1 && py + i >= 0 && !visited[px + j, py + i])
                        {
                            stack.Push((px + j, py + i, pdistance + 2));
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
                x = random.Next(0, Width);
                y = random.Next(GoalAreaHeight, Height - GoalAreaHeight);
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

        // Never used :'(
        public bool AreAnyPiecesInGoalArea()
        {
            for (int y = 0; y < GoalAreaHeight; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (BoardTable[x, y].Piece != null)
                        return true;
                }
            }

            for (int y = Height - GoalAreaHeight; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (BoardTable[x, y].Piece != null)
                        return true;
                }
            }

            return false;
        }

        public object Clone()
        {
            GameMasterBoard gmb = new GameMasterBoard(Height, Width, GoalAreaHeight);
            
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    gmb.BoardTable[i, j] = (GameMasterField)BoardTable[i, j].Clone();
                }
            }
            gmb.PieceCount = PieceCount;

            List<(int x, int y)> piecesTmp = new List<(int x, int y)>(this.PiecesPositions);

            foreach (var pos in piecesTmp)
            {
                gmb.PiecesPositions.Add((pos.x, pos.y));
            }

            return gmb;
        }

    }
}