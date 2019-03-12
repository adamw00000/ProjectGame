namespace GameLib.Actions
{
    internal class ActionCheckPiece : IAction
    {
        public ActionCheckPiece(int agentId)
        {
            AgentId = agentId;
        }

        public int AgentId { get; }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.CheckPiece(AgentId);
        }
    }
}