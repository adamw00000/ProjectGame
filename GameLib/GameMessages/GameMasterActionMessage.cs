using GameLib.GameMessages;

namespace GameLib.Actions
{
    public abstract class GameMasterActionMessage : GameMasterMessage
    {
        public GameMasterActionMessage(int agentId, int timestamp) : base(agentId, timestamp)
        {

        }
    }
}