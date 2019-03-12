namespace GameLib
{
    public class AgentDiscoveryResult
    {
        public (int X, int Y) BasePosition { get; }
        public AgentField[,] Fields { get; }

        public bool IsValid(AgentBoard board) => Fields.GetLength(0) == 3 && Fields.GetLength(1) == 3 &&
            BasePosition.X < board.Width && BasePosition.Y < board.Height && BasePosition.X >= 0 && BasePosition.Y >= 0;

        public AgentDiscoveryResult(int X, int Y, AgentField[,] fields)
        {
            BasePosition = (X, Y);
            Fields = fields;
        }
    }
}