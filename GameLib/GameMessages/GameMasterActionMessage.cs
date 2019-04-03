namespace GameLib
{
    public abstract class GameMasterActionMessage : GameMasterGameMessage
    {
        public GameMasterActionMessage(int agentId, int timestamp) : base(agentId, timestamp)
        {

        }
    }
}