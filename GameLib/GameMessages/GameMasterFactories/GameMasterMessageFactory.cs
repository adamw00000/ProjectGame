using System;
using System.Collections.Generic;
using System.Text;

using ConnectionLib;

namespace GameLib
{
    public class GameMasterMessageFactory : IGameMasterMessageFactory
    {
        public Message CreateCheckPieceResponseMessage(int agentId, int timestamp, int waitUntil, bool result, string messageId)
        {
            return new ActionCheckPieceResponse(agentId, timestamp, waitUntil, result, messageId);
        }

        public Message CreateCommunicationRequestMessage(int requesterAgentId, int targetAgentId, int timestamp)
        {
            return new ActionCommunicationRequest(requesterAgentId, targetAgentId, timestamp);
        }

        public Message CreateCommunicationResponseWithDataMessage(int agentId, int timestamp, int waitUntil, int senderId, bool agreement, object data, string messageId)
        {
            return new ActionCommunicationResponseWithData(agentId, timestamp, waitUntil, senderId, agreement, data, messageId);
        }

        public Message CreateDestoryPieceResponseMessage(int agentId, int timestamp, int waitUntil, string messageId)
        {
            return new ActionDestroyPieceResponse(agentId, timestamp, waitUntil, messageId);
        }

        public Message CreateDiscoveryResponseMessage(int agentId, int timestamp, int waitUntil, DiscoveryResult discoveryResult, string messageId)
        {
            return new ActionDiscoverResponse(agentId, timestamp, waitUntil, discoveryResult, messageId);
        }

        public Message CreateGameOverMessage(int agentId, int timestamp, Team winnerTeam)
        {
            return new GameOverMessage(agentId, timestamp, winnerTeam);
        }

        public Message CreateGameStartMessage(int agentId, long absoluteTimestamp, AgentGameRules agentGameRules)
        {
            return new GameStartMessage(agentId, agentGameRules, absoluteTimestamp);
        }

        public Message CreateInvalidActionErrorMessage(int agentId, int timestamp, string messageId)
        {
            return new InvalidAction(agentId, timestamp, messageId);
        }

        public Message CreateInvalidMoveDirectionErrorMessage(int agentId, int timestamp, string messageId)
        {
            return new InvalidMoveDirectionError(agentId, timestamp, messageId);
        }

        public Message CreateJoinGameResponseMessage(int agentId, bool isConnected)
        {
            return new JoinGameResponseMessage(agentId, isConnected);
        }

        public Message CreateMakeMoveResponseMessage(int agentId, int timestamp, int waitUntil, int distance, string messageId)
        {
            return new ActionMakeMoveResponse(agentId, timestamp, waitUntil, distance, messageId);
        }

        public Message CreatePickPieceResponseMessage(int agentId, int timestamp, int waitUntil, string messageId)
        {
            return new ActionPickPieceResponse(agentId, timestamp, waitUntil, messageId);
        }

        public Message CreatePutPieceResponseMessage(int agentId, int timestamp, int waitUntil, PutPieceResult result, string messageId)
        {
            return new ActionPutPieceResponse(agentId, timestamp, waitUntil, result, messageId);
        }

        public Message CreateTimePenaltyErrorMessage(int agentId, int timestamp, int waitUntil, string messageId)
        {
            return new RequestTimePenaltyError(agentId, timestamp, waitUntil, messageId);
        }
    }
}
