using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.GameMessages.GameSetup
{
    class JoinToGameResponseMessage : GameMasterMessage
    {
        public readonly bool IsConnected;

        public JoinToGameResponseMessage(int agentId, int timestamp, bool isConnected) : base(agentId, timestamp)
        {
            IsConnected = isConnected;
        }

        public override void Handle(object handler)
        {
            ((Agent)handler).HandleJoinResponse(IsConnected);
        }
    }
}
