namespace GameLib.Actions
{
    public interface IAction
    {
        void Handle(GameMaster gameMaster);

        int AgentId { get; }
    }
}