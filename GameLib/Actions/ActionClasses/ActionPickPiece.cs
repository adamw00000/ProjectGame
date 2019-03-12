namespace GameLib.Actions
{
    internal class ActionPickPiece : IAction
    {
        public ActionPickPiece(int agentId)
        {
            AgentId = agentId;
        }

        public int AgentId { get; }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.PickPiece(AgentId);
        }
    }
}