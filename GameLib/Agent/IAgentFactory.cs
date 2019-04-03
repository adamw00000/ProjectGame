using ConnectionLib;
using GameLib;

namespace GameLib
{
    public interface IAgentFactory
    {
        ActionMessage CheckPieceMessage(int agentId, string messageId);
        ActionMessage CommunicationAgreementMessage(int agentId, int senderId, bool acceptsCommunication, object data, string messageId);
        ActionMessage CommunicationRequestMessage(int agentId, int targetId, object data, string messageId);
        ActionMessage DestroyMessage(int agentId, string messageId);
        ActionMessage DiscoveryMessage(int agentId, string messageId);
        ActionMessage MoveMessage(int agentId, MoveDirection direction, string messageId);
        ActionMessage PickPieceMessage(int agentId, string messageId);
        ActionMessage PutPieceMessage(int agentId, string messageId);
        Message JoinGameMessage(Team choosenTeam, bool wantsToBeLeader);
    }
}