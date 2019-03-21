using System;
using GameLib.GameMessages;

namespace GameLib.Actions
{
    internal class ActionDiscoverResponse : GameMasterMessage, IActionResponse
    {

        private readonly DiscoveryResult closestPieces;
        public int WaitUntilTime { get; }

        public ActionDiscoverResponse(int agentId, int timestamp, int waitUntilTime, DiscoveryResult closestPieces) : base(agentId, timestamp)
        {
            this.closestPieces = closestPieces;
            this.WaitUntilTime = waitUntilTime;
        }

        public void Handle(Agent agent)
        {
            agent.HandleDiscoverResponse(Timestamp, WaitUntilTime, closestPieces);
        }
        public override void Handle(object handler)
        {
            Handle((Agent)handler);
        }
    }
}