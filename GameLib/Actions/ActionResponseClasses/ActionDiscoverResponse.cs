using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionDiscoverResponse : GameMasterMessage, IActionResponse
    {

        private readonly int[,] closestPieces;
        public int WaitUntilTime { get; }

        public ActionDiscoverResponse(int agentId, int timestamp, int waitUntilTime, int[,] closestPieces) : base(agentId, timestamp)
        {
            this.closestPieces = closestPieces;
            this.WaitUntilTime = waitUntilTime;
        }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}