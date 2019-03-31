using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    class JoinGameResponseMessage : GameMasterMessage
    {
        public readonly bool IsConnected;

        public JoinGameResponseMessage(int agentId, bool isConnected) : base(agentId, -1)
        {
            IsConnected = isConnected;
        }

        public override void Handle(Agent agent)
        {
            agent.HandleJoinResponse(IsConnected);
        }
    }
}
