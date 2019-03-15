namespace GameLib.Actions
{
    internal class ActionPutPiece : IAction
    {
        public ActionPutPiece(int agentId)
        {
            AgentId = agentId;
        }

        public int AgentId { get; }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.PutPiece(AgentId);
        }
    }
}