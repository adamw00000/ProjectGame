using GameLib.GameMessages;
using System;

namespace GameLib.Actions
{
    internal class ActionCommunicationRequestWithData : AgentMessage, IAction
    {
        private readonly object data;

        public int TargetId { get; }

        public ActionCommunicationRequestWithData(int agentId, int timestamp, int targetId, object data) : base(agentId, timestamp)
        {
            this.data = data;
            TargetId = targetId;
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.CommunicationRequestWithData(AgentId, TargetId, data, Timestamp);
        }

        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}