namespace GameLib
{
    public class GameRules
    {
        public int AgentStartX { get; }
        public int AgentStartY { get; }
        public int[] AgentIdsFromTeam { get; }
        public int TeamLiderId { get; }

        public int BoardWidth { get; }
        public int BoardHeight { get; }
        public int GoalAreaHeight { get; }
        public int GoalCount { get; }
        public int TeamSize { get; }
        public int PieceSpawnInterval { get; }
        public int MaxPiecesOnBoard { get; }
        public double BadPieceProbability { get; }

        public int BaseTimePenalty { get; }
        public int MoveMultiplier { get; }
        public int DiscoverMultiplier { get; }
        public int PickUpMultiplier { get; }
        public int CheckMultiplier { get; }
        public int DestroyPieceMultiplier { get; }
        public int PutPieceMultiplier { get; }
        public int CommunicationMultiplier { get; }

        public GameRules(/*GameStartMessage gsm <- z tego czegoś zostaną odpakowane wartości pól*/)
        {
        }

        public GameRules(int boardWidth = 8, int boardHeight = 8, int goalAreaHeight = 2, int goalCount = 4,
            int teamSize = 5, int pieceSpawnInterval = 500, int maxPiecesOnBoard = 10, double badPieceProbability = 0.5,
            int baseTimePenalty = 50, int moveMultiplier = 1, int discoverMultiplier = 2, int pickUpMultiplier = 2,
            int checkMultiplier = 4, int destroyMultiplier = 4, int putMultiplier = 4, int communicationMultiplier = 4,
            int agentStartX = 4, int agentStartY = 4, int[] agentIdsFromTeam = null, int leaderId = 0)
        {
            AgentStartX = agentStartX;
            AgentStartY = agentStartY;
            AgentIdsFromTeam = agentIdsFromTeam;
            TeamLiderId = leaderId;

            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            GoalAreaHeight = goalAreaHeight;
            GoalCount = goalCount;
            TeamSize = teamSize;
            PieceSpawnInterval = pieceSpawnInterval;
            MaxPiecesOnBoard = maxPiecesOnBoard;
            BadPieceProbability = badPieceProbability;

            BaseTimePenalty = baseTimePenalty;
            MoveMultiplier = moveMultiplier;
            DiscoverMultiplier = discoverMultiplier;
            PickUpMultiplier = pickUpMultiplier;
            CheckMultiplier = checkMultiplier;
            DestroyPieceMultiplier = destroyMultiplier;
            PutPieceMultiplier = putMultiplier;
            CommunicationMultiplier = communicationMultiplier;
        }

        public GameRules ReconstructWithAgentPosition(int x, int y, int[] playersFromTeam, int leaderId)
        {
            return new GameRules(
                boardWidth: BoardWidth,
                boardHeight: BoardHeight,
                goalAreaHeight: GoalAreaHeight,
                goalCount: GoalCount,
                teamSize: TeamSize,
                pieceSpawnInterval: PieceSpawnInterval,
                maxPiecesOnBoard: MaxPiecesOnBoard,
                badPieceProbability: BadPieceProbability,
                baseTimePenalty: BaseTimePenalty,
                moveMultiplier: MoveMultiplier,
                discoverMultiplier: DestroyPieceMultiplier,
                pickUpMultiplier: PickUpMultiplier,
                checkMultiplier: CheckMultiplier,
                destroyMultiplier: DestroyPieceMultiplier,
                putMultiplier: PutPieceMultiplier,
                communicationMultiplier: CommunicationMultiplier,
                agentStartX: x,
                agentStartY: y,
                agentIdsFromTeam: playersFromTeam,
                leaderId: leaderId
            );
        }
    }
}