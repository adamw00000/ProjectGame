using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.GameMessages.GameSetup
{
    class JoinToGameMessage : AgentMessage
    {
        public readonly int TeamId;
        public readonly bool WantToBeLeader;

        public JoinToGameMessage(int agentId, int teamId, bool wantToBeLeader) : base(agentId)
        {
            TeamId = teamId;
            WantToBeLeader = wantToBeLeader;
        }

        public override void Handle(object handler)
        {
            ((GameMaster)handler).JoinToGame(AgentId, TeamId, WantToBeLeader);
        }
    }
}
