namespace GameLib.Actions
{
    internal class ActionDestroyPiece : IAction
    {
        public int AgentId { get; }

        public ActionDestroyPiece(int agentId)
        {
            AgentId = agentId;
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.DestroyPiece(AgentId);
        }
    }
}