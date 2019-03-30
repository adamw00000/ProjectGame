using GameLib.GameMessages;
using System;

namespace GameLib.Actions
{
    internal class ActionCommunicationRequestWithDataMessage : ActionMessage
    {
        private readonly object data;

        public int TargetId { get; }

        public ActionCommunicationRequestWithDataMessage(int agentId, int targetId, object data, string messageId) : base(agentId, messageId)
        {
            this.data = data;
            TargetId = targetId;
        }

        public override void Handle(GameMaster gameMaster)
        {
            gameMaster.CommunicationRequestWithData(AgentId, TargetId, data);
        }
    }
}