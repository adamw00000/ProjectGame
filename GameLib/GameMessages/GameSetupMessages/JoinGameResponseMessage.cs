using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.GameMessages
{
    class JoinGameResponseMessage : GameMasterMessage
    {
        public readonly bool IsConnected;

        public JoinGameResponseMessage(int agentId, int timestamp, bool isConnected) : base(agentId, timestamp)
        {
            IsConnected = isConnected;
        }

        public override void Handle(Agent agent)
        {
            agent.HandleJoinResponse(IsConnected);
        }
    }
}
