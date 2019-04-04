using ConnectionLib;
using GameLib;

namespace GameLib
{
    public interface IAgentMessageFactory
    {
        ActionMessage CreateCheckPieceMessage(int agentId, string messageId);
        ActionMessage CreateCommunicationAgreementMessage(int agentId, int senderId, bool acceptsCommunication, object data, string messageId);
        ActionMessage CreateCommunicationRequestMessage(int agentId, int targetId, object data, string messageId);
        ActionMessage CreateDestroyMessage(int agentId, string messageId);
        ActionMessage CreateDiscoveryMessage(int agentId, string messageId);
        ActionMessage CreateMoveMessage(int agentId, MoveDirection direction, string messageId);
        ActionMessage CreatePickPieceMessage(int agentId, string messageId);
        ActionMessage CreatePutPieceMessage(int agentId, string messageId);
        Message CreateJoinGameMessage(Team choosenTeam, bool wantsToBeLeader);
    }
}