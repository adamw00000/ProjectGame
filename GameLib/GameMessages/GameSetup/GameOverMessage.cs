using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.GameMessages.GameSetup
{
    class GameOverMessage : GameMasterMessage
    {
        public readonly int WinningTeam;

        public GameOverMessage(int agentId, int timestamp, int winningTeam) : base(agentId, timestamp)
        {
            WinningTeam = winningTeam;
        }

        public override void Handle(object handler)
        {
            ((Agent)handler).EndGame(WinningTeam, Timestamp);
        }
    }
}
