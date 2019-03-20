namespace GameLib.Actions
{
    internal class ActionPickPiece : IAction
    {
        public int AgentId { get; }

        public ActionPickPiece(int agentId)
        {
            AgentId = agentId;
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.PickPiece(AgentId);
        }
    }
}