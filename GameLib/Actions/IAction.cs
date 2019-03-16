namespace GameLib.Actions
{
    public interface IAction
    {
        int AgentId { get; }
        void Handle(GameMaster gameMaster);
    }
}