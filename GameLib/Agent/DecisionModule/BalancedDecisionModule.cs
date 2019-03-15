using GameLib.Actions;
using System;

namespace GameLib
{
    // Jeden z przykładów klasy która implementuje IDecisionModule
    public class BalancedDecisionModule : IDecisionModule
    {
        // Tutaj jakiś inner state modułu, poprzednie decyzje itd, rzeczy które mogą wpłynąć na kolejny ruch

        public IAction ChooseAction(AgentState agentState)
        {
            throw new NotImplementedException();
        }
    }
}