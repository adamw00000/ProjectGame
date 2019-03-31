using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    class GameStartMessage : GameMasterMessage
    {
        public readonly AgentGameRules Rules;
        public readonly long AbsoluteStart;

        public GameStartMessage(int agentId, AgentGameRules rules, long absStart) : base(agentId, -1)
        {
            Rules = rules;
            AbsoluteStart = absStart;
        }
        public override void Handle(Agent agent)
        {
            agent.HandleStartGameMessage(AgentId, Rules, AbsoluteStart);
        }
    }
}
