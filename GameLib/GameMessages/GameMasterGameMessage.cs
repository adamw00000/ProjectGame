namespace GameLib
{
    public abstract class GameMasterGameMessage : GameMasterMessage
    {
        public readonly int Timestamp;

        public GameMasterGameMessage(int agentId, int timestamp) : base(agentId)
        {
            Timestamp = timestamp;
        }
    }
}