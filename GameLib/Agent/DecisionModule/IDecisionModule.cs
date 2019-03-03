using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public interface IDecisionModule
    {
        IAction ChooseAction(AgentState agentState);
    }
}
