using ConnectionLib;
using GameLib.Actions;

namespace GameLib
{
    public class AgentFactory : IAgentFactory
    {
        public ActionMessage CheckPieceMessage(int agentId, string messageId)
        {
            return new ActionPickPieceMessage(agentId, messageId);
        }

        public ActionMessage CommunicationAgreementMessage(int agentId, int senderId, bool acceptsCommunication, object data, string messageId)
        {
            return new ActionCommunicationAgreementWithDataMessage(agentId, senderId, acceptsCommunication, data, messageId);
        }

        public ActionMessage CommunicationRequestMessage(int agentId, int targetId, object data, string messageId)
        {
            return new ActionCommunicationRequestWithDataMessage(agentId, targetId, data, messageId);
        }

        public ActionMessage DestroyMessage(int agentId, string messageId)
        {
            return new ActionDestroyPieceMessage(agentId, messageId);
        }

        public ActionMessage DiscoveryMessage(int agentId, string messageId)
        {
            return new ActionDiscoveryMessage(agentId, messageId);
        }

        public ActionMessage MoveMessage(int agentId, MoveDirection direction, string messageId)
        {
            return new ActionMoveMessage(agentId, direction, messageId);
        }

        public ActionMessage PickPieceMessage(int agentId, string messageId)
        {
            return new ActionPickPieceMessage(agentId, messageId);
        }

        public ActionMessage PutPieceMessage(int agentId, string messageId)
        {
            return new ActionPutPieceMessage(agentId, messageId);
        }
    }
}
