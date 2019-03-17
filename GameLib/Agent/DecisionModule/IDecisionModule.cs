using GameLib.Actions;

namespace GameLib
{
    public interface IDecisionModule
    {
        IAction ChooseAction(int agentId, AgentState agentState);
    }
}