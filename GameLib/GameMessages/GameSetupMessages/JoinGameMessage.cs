using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    class JoinGameMessage : AgentMessage
    {
        public readonly int TeamId;
        public readonly bool WantToBeLeader;

        public JoinGameMessage(int agentId, int teamId, bool wantToBeLeader) : base(agentId)
        {
            TeamId = teamId;
            WantToBeLeader = wantToBeLeader;
        }

        public override void Handle(GameMaster gameMaster)
        {
            gameMaster.JoinGame(AgentId, TeamId, WantToBeLeader);
        }

        public override string ToString()
        {
            return $"JoinGameMessage (agentId: {AgentId}, teamId: {TeamId}, does agent want to be a leader: {WantToBeLeader})";
        }
    }
}
