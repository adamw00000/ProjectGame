using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.GameMessages
{
    class GameStartMessage : GameMasterMessage
    {
        public readonly GameRules Rules;
        public readonly long AbsoluteStart;

        public GameStartMessage(int agentId, int timestamp, GameRules rules, long absStart) : base(agentId, timestamp)
        {
            Rules = rules;
            AbsoluteStart = absStart;
        }
        public override void Handle(object handler)
        {
            ((Agent)handler).HandleStartGameMessage(AgentId, Rules, Timestamp, AbsoluteStart);
        }
    }
}
