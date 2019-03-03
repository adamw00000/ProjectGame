namespace GameLib
{
    public class GameMasterBoard
    {
        public readonly GameMasterField[,] Board;

        public GameMasterBoard(int width, int height)
        {
            Board = new GameMasterField[width, height];
        }
    }
}