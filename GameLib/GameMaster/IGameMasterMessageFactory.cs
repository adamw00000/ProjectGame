using System;
using System.Collections.Generic;
using System.Text;

using ConnectionLib;

namespace GameLib
{
    public interface IGameMasterMessageFactory
    {
        // Game Setup
        Message CreateJoinGameResponseMessage(int agentId, bool isConnected);
        Message CreateGameStartMessage(int agentId, long absoluteTimestamp, AgentGameRules agentGameRules);
        Message CreateGameOverMessage(int agentId, int timestamp, Team winnerTeam);

        // Action Responses
        Message CreateMoveResponseMessage(int agentId, int timestamp, int waitUntil, int distance, string messageId);
        Message CreatePickPieceResponseMessage(int agentId, int timestamp, int waitUntil, string messageId);
        Message CreatePutPieceResponseMessage(int agentId, int timestamp, int waitUntil, PutPieceResult result, string messageId);
        Message CreateDiscoveryResponseMessage(int agentId, int timestamp, int waitUntil, DiscoveryResult discoveryResult, string messageId);
        Message CreateCheckPieceResponseMessage(int agentId, int timestamp, int waitUntil, bool result, string messageId);
        Message CreateDestroyPieceResponseMessage(int agentId, int timestamp, int waitUntil, string messageId);
        Message CreateCommunicationResponseWithDataMessage(int agentId, int timestamp, int waitUntil, int senderId, bool agreement, object data, string messageId);

        // Game Master Actions
        Message CreateCommunicationRequestMessage(int requesterAgentId, int targetAgentId, int timestamp);

        // Action Errors
        Message CreateInvalidActionErrorMessage(int agentId, int timestamp, string messageId);
        Message CreateTimePenaltyErrorMessage(int agentId, int timestamp, int waitUntil, string messageId);
        Message CreateInvalidMoveDirectionErrorMessage(int agentId, int timestamp, string messageId);
    }
}
