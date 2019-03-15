using GameLib.Actions;

namespace GameLib
{
    public interface IDecisionModule
    {
        IAction ChooseAction(AgentState agentState);
    }
}