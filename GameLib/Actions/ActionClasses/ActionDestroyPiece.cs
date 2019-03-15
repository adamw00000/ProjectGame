namespace GameLib.Actions
{
    internal class ActionDestroyPiece : IAction
    {
        public ActionDestroyPiece(int agentId)
        {
            AgentId = agentId;
        }

        public int AgentId { get; }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.DestroyPiece(AgentId);
        }
    }
}