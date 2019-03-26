using Newtonsoft.Json;

namespace GameLib
{
    public class GameRules
    {
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
        public int PickUpPieceMultiplier { get; }
        public int CheckPieceMultiplier { get; }
        public int DestroyPieceMultiplier { get; }
        public int PutPieceMultiplier { get; }
        public int CommunicationMultiplier { get; }

        public GameRules(/*GameStartMessage gsm <- z tego czegoś zostaną odpakowane wartości pól*/)
        {
        }

        public GameRules(int boardWidth = 8, int boardHeight = 8, int goalAreaHeight = 2, int goalCount = 4,
            int teamSize = 5, int pieceSpawnInterval = 500, int maxPiecesOnBoard = 10, double badPieceProbability = 0.5,
            int baseTimePenalty = 50, int moveMultiplier = 1, int discoverMultiplier = 2, int pickUpMultiplier = 2,
            int checkMultiplier = 4, int destroyMultiplier = 4, int putMultiplier = 4, int communicationMultiplier = 4)
        {
            if (boardWidth < 1)
                throw new InvalidRulesException("Invalid board width value");
            if (boardHeight < 1)
                throw new InvalidRulesException("Invalid board height value");
            if (goalAreaHeight < 1 || goalAreaHeight * 2 >= boardHeight)
                throw new InvalidRulesException("Invalid goal area height value");
            int taskAreaSize = boardWidth * (boardHeight - 2 * goalAreaHeight);
            int goalAreaSize = boardWidth * goalAreaHeight;
            if (goalCount < 1 || goalCount > goalAreaSize)
                throw new InvalidRulesException("Invalid goal count value");
            if (teamSize < 1 || teamSize > goalAreaSize)
                throw new InvalidRulesException("Invali team size value");
            if (pieceSpawnInterval < 0)
                throw new InvalidRulesException("Invalid spawn interval value");
            if (maxPiecesOnBoard < 1 || maxPiecesOnBoard > taskAreaSize)
                throw new InvalidRulesException("Invalid max pieces on board value");
            if (badPieceProbability < 0.0 || badPieceProbability >= 1.0)
                throw new InvalidRulesException("Invalid bad piece probability");
            if (baseTimePenalty < 1)
                throw new InvalidRulesException("Invalid base time penalty");
            if (moveMultiplier < 1)
                throw new InvalidRulesException("Invalid move multiplier");
            if (discoverMultiplier < 1)
                throw new InvalidRulesException("Invalid discover multiplier");
            if (pickUpMultiplier < 1)
                throw new InvalidRulesException("Invalid pick up piece multiplier");
            if (checkMultiplier < 1)
                throw new InvalidRulesException("Invalid check piece multiplier");
            if (destroyMultiplier < 1)
                throw new InvalidRulesException("Invalid destroy piece multiplier");
            if (putMultiplier < 1)
                throw new InvalidRulesException("Invalid put piece multiplier");
            if (communicationMultiplier < 1)
                throw new InvalidRulesException("Invalid communication multiplier");

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
            PickUpPieceMultiplier = pickUpMultiplier;
            CheckPieceMultiplier = checkMultiplier;
            DestroyPieceMultiplier = destroyMultiplier;
            PutPieceMultiplier = putMultiplier;
            CommunicationMultiplier = communicationMultiplier;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}