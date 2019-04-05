using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace GameLib.Tests
{
    public class GameRulesTests
    {
        [Theory]
        [InlineData(-1, 2, 1)]
        [InlineData(2, -2, 1)]
        [InlineData(0, 0, 0)]
        [InlineData(4, 4, 0)]
        [InlineData(2, 4, 2)]
        [InlineData(4, 5, 3)]
        public void GameRules_InvalidBoardSize_ThrowsException(int boardWidth, int boardHeight, int goalAreaHeight)
        {
            GameRules rules;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(boardWidth: boardWidth, boardHeight: boardHeight, goalAreaHeight: goalAreaHeight));
        }

        [Theory]
        [InlineData(3, 2, -6)]
        [InlineData(4, 2, 10)]
        public void GameRules_InvalidTeamSize_ThrowsException(int boardWidth, int goalAreaHeight, int teamSize)
        {
            GameRules rules;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(boardWidth: boardWidth, goalAreaHeight: goalAreaHeight, teamSize: teamSize));
        }

        [Theory]
        [InlineData(4, 1, 0)]
        [InlineData(3, 1, 5)]
        public void GameRules_InvalidGoalCount_ThrowsException(int boardWidth, int goalAreaHeight, int goalCount)
        {
            GameRules rules;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(boardWidth: boardWidth, goalAreaHeight: goalAreaHeight, goalCount: goalCount));
        }

        [Theory]
        [InlineData(-2)]
        public void GameRules_InvalidPieceSpawnInterval_ThrowsException(int pieceSpawnInterval)
        {
            GameRules rules;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(pieceSpawnInterval: pieceSpawnInterval));
        }

        [Theory]
        [InlineData(2, 4, 1, 0)]
        [InlineData(3, 3, 1, -3)]
        [InlineData(4, 6, 2, 20)]
        public void GameRules_InvalidMaxPieceOnBoard_ThrowsException(int boardWidth, int boardHeight, int goalAreaHeight, int maxPiecesOnBoard)
        {
            GameRules rules;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(boardWidth: boardWidth, boardHeight: boardHeight, goalAreaHeight: goalAreaHeight, maxPiecesOnBoard: maxPiecesOnBoard));
        }

        [Theory]
        [InlineData(-0.3)]
        [InlineData(1.0)]
        [InlineData(3.2)]
        public void GameRules_InvalidBadPieceProbability_ThrowsException(double badPieceProbability)
        {
            GameRules rules;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(badPieceProbability: badPieceProbability));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(0)]
        public void GameRules_InvalidBaseTimePenalty_ThrowsException(int baseTimePenalty)
        {
            GameRules rules;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(baseTimePenalty: baseTimePenalty));
        }

        [Fact]
        public void GameRules_InvalidMoveMultiplier_ThrowsException()
        {
            GameRules rules;
            int moveMultiplier = -1;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(moveMultiplier: moveMultiplier));
        }

        [Fact]
        public void GameRules_InvalidDiscoveryMultiplier_ThrowsException()
        {
            GameRules rules;
            int discoverMultiplier = -2;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(discoverMultiplier: discoverMultiplier));
        }

        [Fact]
        public void GameRules_InvalidPickUpMultiplier_ThrowsException()
        {
            GameRules rules;
            int pickUpMultiplier = -1;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(pickUpMultiplier: pickUpMultiplier));
        }

        [Fact]
        public void GameRules_InvalidCheckMultiplier_ThrowsException()
        {
            GameRules rules;
            int checkMultiplier = -2;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(checkMultiplier: checkMultiplier));
        }

        [Fact]
        public void GameRules_InvalidDestroyMultiplier_ThrowsException()
        {
            GameRules rules;
            int destroyMultiplier = -1;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(destroyMultiplier: destroyMultiplier));
        }

        [Fact]
        public void GameRules_InvalidPutMultiplier_ThrowsException()
        {
            GameRules rules;
            int putMultiplier = -2;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(putMultiplier: putMultiplier));
        }

        [Fact]
        public void GameRules_InvalidCommunicationMultiplier_ThrowsException()
        {
            GameRules rules;
            int communicationMultiplier = -1;

            Should.Throw<InvalidRulesException>(() => rules = new GameRules(communicationMultiplier: communicationMultiplier));
        }

        //-----------------------AgentGameRules-----------------------

        [Theory]
        [InlineData(1, -1)]
        [InlineData(8, 1)]
        [InlineData(1, 8)]
        [InlineData(-1, 6)]
        public void AgentGameRules_WhenAgentStartsOutsideTheBoard_ThrowsException(int x, int y)
        {
            GameRules rules;

            Should.Throw<InvalidRulesException>(() => rules = new AgentGameRules(agentStartX: x, agentStartY: y));
        }

        [Theory]
        [InlineData(4, 4)]
        [InlineData(2, 5)]
        [InlineData(5, 1)]
        public void AgentGameRules_WhenAgentStartsInTaskArea_ThrowsException(int x, int y)
        {
            GameRules rules;

            Should.Throw<InvalidRulesException>(() => rules = new AgentGameRules(agentStartX: x, agentStartY: y));
        }

        [Fact]
        public void AgentGameRules_IdsArrayIsNull_ThrownsException()
        {
            AgentGameRules rules;
            int[] agentIds = null;

            Should.Throw<InvalidRulesException>(() => rules = new AgentGameRules(agentIdsFromTeam: agentIds));
        }

        [Fact]
        public void AgentGameRules_IdsArrayLenghtIsDifferentFromTeamSize_ThrownsException()
        {
            AgentGameRules rules;
            int[] agentIds = new int[] { 0, 1, 2, 3 };

            Should.Throw<InvalidRulesException>(() => rules = new AgentGameRules(agentIdsFromTeam: agentIds));
        }

        [Fact]
        public void AgentGameRules_IdsArrayContainsDuplicates_ThrownsException()
        {
            AgentGameRules rules;
            int[] agentIds = new int[] { 0, 2, 2, 3, 3 };

            Should.Throw<InvalidRulesException>(() => rules = new AgentGameRules(agentIdsFromTeam: agentIds));
        }

        [Fact]
        public void AgentGameRules_IdsArrayDoesntContainLeaderId_ThrownsException()
        {
            AgentGameRules rules;
            int[] agentIds = new int[] { 0, 1, 2, 3, 4 };
            int leaderId = 5;

            Should.Throw<InvalidRulesException>(() => rules = new AgentGameRules(agentIdsFromTeam: agentIds, leaderId: leaderId));
        }
    }
}
