using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.GameMessages
{
    class JoinGameMessage : AgentMessage
    {
        public readonly int TeamId;
        public readonly bool WantToBeLeader;

        public JoinGameMessage(int agentId, int teamId, bool wantToBeLeader) : base(agentId, 0)
        {
            TeamId = teamId;
            WantToBeLeader = wantToBeLeader;
        }

        public override void Handle(object handler)
        {
            ((GameMaster)handler).JoinGame(AgentId, TeamId, WantToBeLeader);
        }
    }
}
