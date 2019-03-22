using GameLib.Actions;
using System;
using System.Threading.Tasks;

namespace GameLib
{
    public interface IDecisionModule
    {
        CommunicationDataProcessor DataProcessor { get; }

        Task<IAction> ChooseAction(int agentId, AgentState agentState);
        void SaveCommunicationResult(int senderId, bool agreement, DateTime timestamp, object data, AgentState agentState);
    }
}