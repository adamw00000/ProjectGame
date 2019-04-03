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

        private string GetMessageId() => Guid.NewGuid().ToString();

        public (Message message, string messageId) MoveMessage(MoveDirection direction)
        {
            string messageId = GetMessageId();
            return (agentFactory.MoveMessage(id, direction, messageId), messageId);
        }

        public (Message message, string messageId) CheckPieceMessage()
        {
            string messageId = GetMessageId();
            return (agentFactory.CheckPieceMessage(id, messageId), messageId);
        }

        public (Message message, string messageId) CommunicationAgreementMessage(int senderId, bool acceptsCommunication, object data)
        {
            string messageId = GetMessageId();
            return (agentFactory.CommunicationAgreementMessage(id, senderId, acceptsCommunication, data, messageId), messageId);
        }

        public (Message message, string messageId) CommunicationRequestMessage(int targetId, object data)
        {
            string messageId = GetMessageId();
            return (agentFactory.CommunicationRequestMessage(id, targetId, data, messageId), messageId);
        }

        public (Message message, string messageId) DestroyMessage()
        {
            string messageId = GetMessageId();
            return (agentFactory.DestroyMessage(id, messageId), messageId);
        }

        public (Message message, string messageId) DiscoveryMessage()
        {
            string messageId = GetMessageId();
            return (agentFactory.DiscoveryMessage(id, messageId), messageId);
        }

        public (Message message, string messageId) PickPieceMessage()
        {
            string messageId = GetMessageId();
            return (agentFactory.PickPieceMessage(id, messageId), messageId);
        }

        public (Message message, string messageId) PutPieceMessage()
        {
            string messageId = GetMessageId();
            return (agentFactory.PutPieceMessage(id, messageId), messageId);
        }
    }
}