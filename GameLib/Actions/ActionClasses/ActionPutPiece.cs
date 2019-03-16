namespace GameLib.Actions
{
    internal class ActionPutPiece : IAction
    {
        public int AgentId { get; }

        public ActionPutPiece(int agentId)
        {
            AgentId = agentId;
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.PutPiece(AgentId);
        }
    }
}