using System;
using System.Threading.Tasks;

namespace GameLib
{
    // Jeden z przykładów klasy która implementuje DecisionModuleBase
    public class BalancedDecisionModule : DecisionModuleBase
    {
        // Tutaj jakiś inner state modułu, poprzednie decyzje itd, rzeczy które mogą wpłynąć na kolejny ruch

        public override Task<Action> ChooseAction(int agentId, AgentState agentState)
        {
            throw new NotImplementedException();
        }

    }
}