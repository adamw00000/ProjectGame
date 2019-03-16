namespace GameLib.Actions
{
    internal class ActionCheckPiece : IAction
    {
        public int AgentId { get; }

        public ActionCheckPiece(int agentId)
        {
            AgentId = agentId;
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.CheckPiece(AgentId);
        }
    }
}