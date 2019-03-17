using ConnectionLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.GameMessages
{
    abstract class GameMasterMessage : Message
    {
        public readonly int Timestamp;

        public GameMasterMessage(int agentId, int timestamp) : base(agentId)
        {
            Timestamp = timestamp;
        }
    }
}
