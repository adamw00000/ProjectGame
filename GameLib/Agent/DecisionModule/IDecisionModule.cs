using GameLib.Actions;
using System.Threading.Tasks;

namespace GameLib
{
    public interface IDecisionModule
    {
        Task<IAction> ChooseAction(int agentId, AgentState agentState);
    }
}