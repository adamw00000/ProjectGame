using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.GameMessages
{
    class GameStartMessage : GameMasterMessage
    {
        public readonly AgentGameRules Rules;
        public readonly long AbsoluteStart;

        public GameStartMessage(int agentId, int timestamp, AgentGameRules rules, long absStart) : base(agentId, timestamp)
        {
            Rules = rules;
            AbsoluteStart = absStart;
        }
        public override void Handle(Agent agent)
        {
            agent.HandleStartGameMessage(AgentId, Rules, Timestamp, AbsoluteStart);
        }
    }
}
