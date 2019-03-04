namespace GameLib
{
    public class CommunicationResult
    {
        public AgentBoard Board { get; }

        public bool IsValid(AgentBoard board) => Board.Width == board.Width && Board.Height == board.Height;

        public CommunicationResult(AgentBoard board)
        {
            Board = board;
        }
    }
}