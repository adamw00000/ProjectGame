using GameLib.GameMessages;
using System;

namespace GameLib.Actions
{
    internal class ActionCommunicationAgreementWithData : AgentMessage, IAction
    {
        private readonly object data;

        public int TargetId { get; }
        public bool AcceptsCommunication { get; }

        public ActionCommunicationAgreementWithData(int agentId, int timestamp, int targetId, bool acceptsCommunication, object data = null) : base(agentId, timestamp)
        {
            TargetId = targetId;
            AcceptsCommunication = acceptsCommunication;
            this.data = data;
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.CommunicationAgreementWithData(AgentId, TargetId, AcceptsCommunication, data, Timestamp);
        }

        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}