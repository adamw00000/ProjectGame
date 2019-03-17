﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.GameMessages.GameSetup
{
    class GameStartMessage : GameMasterMessage
    {
        public readonly GameRules Rules;

        public GameStartMessage(int agentId, int timestamp, GameRules rules) : base(agentId, timestamp)
        {
            Rules = rules;
        }
        public override void Handle(object handler)
        {
            ((Agent)handler).StartGame(Rules, Timestamp);
        }
    }
}
