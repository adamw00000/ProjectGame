using ConnectionLib;
using GameLib;
using System;

namespace GameLib
{
    internal class AgentMessageFactoryWrapper
    {
        private readonly IAgentMessageFactory agentFactory;
        private readonly int id;

        public AgentMessageFactoryWrapper(int id, IAgentMessageFactory agentFactory)
        {
            this.agentFactory = agentFactory;
            this.id = id;
        }

        private string GetMessageId() => Guid.NewGuid().ToString();

        public (Message message, string messageId) MoveMessage(MoveDirection direction)
        {
            string messageId = GetMessageId();
            return (agentFactory.CreateMoveMessage(id, direction, messageId), messageId);
        }

        public (Message message, string messageId) CheckPieceMessage()
        {
            string messageId = GetMessageId();
            return (agentFactory.CreateCheckPieceMessage(id, messageId), messageId);
        }

        public (Message message, string messageId) CommunicationAgreementMessage(int senderId, bool acceptsCommunication, object data)
        {
            string messageId = GetMessageId();
            return (agentFactory.CreateCommunicationAgreementMessage(id, senderId, acceptsCommunication, data, messageId), messageId);
        }

        public (Message message, string messageId) CommunicationRequestMessage(int targetId, object data)
        {
            string messageId = GetMessageId();
            return (agentFactory.CreateCommunicationRequestMessage(id, targetId, data, messageId), messageId);
        }

        public (Message message, string messageId) DestroyMessage()
        {
            string messageId = GetMessageId();
            return (agentFactory.CreateDestroyMessage(id, messageId), messageId);
        }

        public (Message message, string messageId) DiscoveryMessage()
        {
            string messageId = GetMessageId();
            return (agentFactory.CreateDiscoveryMessage(id, messageId), messageId);
        }

        public (Message message, string messageId) PickPieceMessage()
        {
            string messageId = GetMessageId();
            return (agentFactory.CreatePickPieceMessage(id, messageId), messageId);
        }

        public (Message message, string messageId) PutPieceMessage()
        {
            string messageId = GetMessageId();
            return (agentFactory.CreatePutPieceMessage(id, messageId), messageId);
        }
    }
}