using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class AgentGameRules : GameRules
    {
        public int AgentStartX { get; }
        public int AgentStartY { get; }
        public int[] AgentIdsFromTeam { get; }
        public int TeamLeaderId { get; }

        public AgentGameRules(int boardWidth = 8, int boardHeight = 8, int goalAreaHeight = 2, int goalCount = 4,
            int teamSize = 5, int pieceSpawnInterval = 500, int maxPiecesOnBoard = 10, double badPieceProbability = 0.5,
            int baseTimePenalty = 50, int moveMultiplier = 1, int discoverMultiplier = 2, int pickUpMultiplier = 2,
            int checkMultiplier = 4, int destroyMultiplier = 4, int putMultiplier = 4, int communicationMultiplier = 4,
            int agentStartX = 0, int agentStartY = 0, int[] agentIdsFromTeam = null, int leaderId = 0) : 
            base(boardWidth, boardHeight, goalAreaHeight, goalAreaHeight, teamSize, pieceSpawnInterval,
                maxPiecesOnBoard, badPieceProbability, baseTimePenalty, moveMultiplier, discoverMultiplier,
                pickUpMultiplier, checkMultiplier, destroyMultiplier, putMultiplier, communicationMultiplier)
        {
            if (agentStartY < 0 || agentStartY >= boardWidth)
                throw new InvalidRulesException("Invalid starting y");
            if (agentStartX < 0 || agentStartX >= boardHeight)
                throw new InvalidRulesException("Invalid starting x");
            if (agentStartX >= goalAreaHeight && agentStartX < boardHeight - goalAreaHeight) //in task area
                throw new InvalidRulesException("Agent starts in task area");
            if (agentIdsFromTeam == null)
                throw new InvalidRulesException("Ids array is null");
            if (agentIdsFromTeam.Length != teamSize)
                throw new InvalidRulesException("Ids array size is different from team size");
            HashSet<int> setId = new HashSet<int>(agentIdsFromTeam);
            if (setId.Count != agentIdsFromTeam.Length)
                throw new InvalidRulesException("Ids array contains duplicates");
            if (!setId.Contains(leaderId))
                throw new InvalidRulesException("Ids array does not contain leader");

            AgentStartX = agentStartX;
            AgentStartY = agentStartY;
            AgentIdsFromTeam = agentIdsFromTeam;
            TeamLeaderId = leaderId;
        }

        public AgentGameRules(GameRules rules, int agentStartX = 4, int agentStartY = 4, int[] agentIdsFromTeam = null, int leaderId = 0) : 
            base(rules.BoardWidth, rules.BoardHeight, rules.GoalAreaHeight, rules.GoalCount, rules.TeamSize, rules.PieceSpawnInterval,
                rules.MaxPiecesOnBoard, rules.BadPieceProbability, rules.BaseTimePenalty, rules.MoveMultiplier, rules.DiscoverMultiplier,
                rules.PickUpPieceMultiplier, rules.CheckPieceMultiplier, rules.DestroyPieceMultiplier, rules.PutPieceMultiplier, rules.CommunicationMultiplier)
        {
            if (agentStartY < 0 || agentStartY >= rules.BoardWidth)
                throw new InvalidRulesException("Invalid starting y");
            if (agentStartX < 0 || agentStartX >= rules.BoardHeight)
                throw new InvalidRulesException("Invalid starting x");
            if (agentStartX >= rules.GoalAreaHeight && agentStartX < rules.BoardHeight - rules.GoalAreaHeight) //in task area
                throw new InvalidRulesException("Agent starts in task area");
            if (agentIdsFromTeam == null)
                throw new InvalidRulesException("Ids array is null");
            if (agentIdsFromTeam.Length != rules.TeamSize)
                throw new InvalidRulesException("Ids array size is different from team size");
            HashSet<int> setId = new HashSet<int>(agentIdsFromTeam);
            if (setId.Count != agentIdsFromTeam.Length)
                throw new InvalidRulesException("Ids array contains duplicates");
            if (!setId.Contains(leaderId))
                throw new InvalidRulesException("Ids array does not contain leader");

            AgentStartX = agentStartX;
            AgentStartY = agentStartY;
            AgentIdsFromTeam = agentIdsFromTeam;
            TeamLeaderId = leaderId;
        }
    }
}
