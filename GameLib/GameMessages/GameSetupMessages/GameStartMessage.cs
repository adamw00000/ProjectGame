using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    class GameStartMessage : GameMasterMessage
    {
        public readonly AgentGameRules Rules;
        public readonly long AbsoluteStart;

        public GameStartMessage(int agentId, AgentGameRules rules, long absStart) : base(agentId)
        {
            Rules = rules;
            AbsoluteStart = absStart;
        }

        public override void Handle(Agent agent)
        {
            agent.HandleStartGameMessage(AgentId, Rules, AbsoluteStart);
        }

        public override string ToString()
        {
            return $"GameStartMessage (agentId: {AgentId}, rules: {Rules}, absolute start timestamp: {AbsoluteStart})";
        }
    }
}
