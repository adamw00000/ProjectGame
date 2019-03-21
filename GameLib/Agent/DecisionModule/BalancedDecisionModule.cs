using GameLib.Actions;
using System;
using System.Threading.Tasks;

namespace GameLib
{
    // Jeden z przykładów klasy która implementuje IDecisionModule
    public class BalancedDecisionModule : IDecisionModule
    {
        public CommunicationDataProcessor DataProcessor { get; } = new CommunicationDataProcessor();

        // Tutaj jakiś inner state modułu, poprzednie decyzje itd, rzeczy które mogą wpłynąć na kolejny ruch

        public Task<IAction> ChooseAction(int agentId, AgentState agentState)
        {
            throw new NotImplementedException();
        }

        public void SaveCommunicationResult(int senderId, bool agreement, DateTime timestamp, object data, AgentState agentState)
        {
            DataProcessor.ExtractCommunicationData(senderId, agreement, timestamp, data, agentState);
        }
    }
}