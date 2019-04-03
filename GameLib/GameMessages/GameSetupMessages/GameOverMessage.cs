using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    class GameOverMessage : GameMasterGameMessage
    {
        public readonly Team WinningTeam;

        public GameOverMessage(int agentId, int timestamp, Team winningTeam) : base(agentId, timestamp)
        {
            WinningTeam = winningTeam;
        }

        public override void Handle(Agent agent)
        {
            agent.EndGame(WinningTeam, Timestamp);
        }
    }
}
