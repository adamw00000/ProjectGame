using ConnectionLib;
using GameLib;

namespace GameLib
{
    public class AgentMessageFactory : IAgentMessageFactory
    {
        public ActionMessage CreateCheckPieceMessage(int agentId, string messageId)
        {
            return new ActionPickPieceMessage(agentId, messageId);
        }

        public ActionMessage CreateCommunicationAgreementMessage(int agentId, int senderId, bool acceptsCommunication, object data, string messageId)
        {
            return new ActionCommunicationAgreementWithDataMessage(agentId, senderId, acceptsCommunication, data, messageId);
        }

        public ActionMessage CreateCommunicationRequestMessage(int agentId, int targetId, object data, string messageId)
        {
            return new ActionCommunicationRequestWithDataMessage(agentId, targetId, data, messageId);
        }

        public ActionMessage CreateDestroyMessage(int agentId, string messageId)
        {
            return new ActionDestroyPieceMessage(agentId, messageId);
        }

        public ActionMessage CreateDiscoveryMessage(int agentId, string messageId)
        {
            return new ActionDiscoveryMessage(agentId, messageId);
        }

        public ActionMessage CreateMoveMessage(int agentId, MoveDirection direction, string messageId)
        {
            return new ActionMoveMessage(agentId, direction, messageId);
        }

        public ActionMessage CreatePickPieceMessage(int agentId, string messageId)
        {
            return new ActionPickPieceMessage(agentId, messageId);
        }

        public ActionMessage CreatePutPieceMessage(int agentId, string messageId)
        {
            return new ActionPutPieceMessage(agentId, messageId);
        }

        public Message CreateJoinGameMessage(Team choosenTeam, bool wantsToBeLeader)
        {
            return new JoinGameMessage(0, (int)choosenTeam, wantsToBeLeader);
        }
    }
}
