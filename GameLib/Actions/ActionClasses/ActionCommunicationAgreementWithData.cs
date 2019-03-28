using GameLib.GameMessages;
using System;

namespace GameLib.Actions
{
    internal class ActionCommunicationAgreementWithData : AgentMessage, IAction
    {
        private readonly object data;

        public int SenderId { get; }
        public bool AcceptsCommunication { get; }

        public ActionCommunicationAgreementWithData(int agentId, int timestamp, int senderId, bool acceptsCommunication, object data = null) : base(agentId, timestamp)
        {
            SenderId = senderId;
            AcceptsCommunication = acceptsCommunication;
            this.data = data;
        }

        public void Handle(GameMaster gameMaster)
        {
            gameMaster.CommunicationAgreementWithData(SenderId, AgentId, AcceptsCommunication, data, Timestamp);
        }

        public override void Handle(object handler)
        {
            Handle((GameMaster)handler);
        }
    }
}