using ConnectionLib;
using GameLib.Actions;
using System;

namespace GameLib
{
    internal class AgentFactoryWrapper
    {
        private readonly IAgentFactory agentFactory;
        private readonly int id;

        public AgentFactoryWrapper(int id, IAgentFactory agentFactory)
        {
            this.agentFactory = agentFactory;
            this.id = id;
        }
        private string GetMessageId()
        {
            return Guid.NewGuid().ToString();
        }

        public ActionMessage MoveMessage(MoveDirection direction)
        {
            string messageId = GetMessageId();
            return agentFactory.MoveMessage(id, direction, messageId);
        }

        public ActionMessage CheckPieceMessage()
        {
            string messageId = GetMessageId();
            return agentFactory.CheckPieceMessage(id, messageId);
        }

        public ActionMessage CommunicationAgreementMessage(int senderId, bool acceptsCommunication, object data)
        {
            string messageId = GetMessageId();
            return agentFactory.CommunicationAgreementMessage(id, senderId, acceptsCommunication, data, messageId);
        }

        public ActionMessage CommunicationRequestMessage(int targetId, object data)
        {
            string messageId = GetMessageId();
            return agentFactory.CommunicationRequestMessage(id, targetId, data, messageId);
        }

        public ActionMessage DestroyMessage()
        {
            string messageId = GetMessageId();
            return agentFactory.DestroyMessage(id, messageId);
        }

        public ActionMessage DiscoveryMessage()
        {
            string messageId = GetMessageId();
            return agentFactory.DiscoveryMessage(id, messageId);
        }

        public ActionMessage PickPieceMessage()
        {
            string messageId = GetMessageId();
            return agentFactory.PickPieceMessage(id, messageId);
        }

        public ActionMessage PutPieceMessage()
        {
            string messageId = GetMessageId();
            return agentFactory.PutPieceMessage(id, messageId);
        }
    }
}