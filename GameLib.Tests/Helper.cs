namespace GameLib.Tests
{
    internal static class Helper
    {
        public static GameRules GetDefaultRules() =>
            new GameRules(boardWidth: 8, boardHeight: 8, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5);

        public static GameRules GetOddSizeBoardRules() =>
            new GameRules(boardWidth: 7, boardHeight: 7, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5); //odd-sized board

        public static GameRules GetStaticDefaultRules() =>
            new GameRules(boardWidth: 8, boardHeight: 8, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5); //Nie ruszac

        public static GameRules GetAlwaysInvalidPieceRules() =>
           new GameRules(boardWidth: 8, boardHeight: 8, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5, pieceSpawnInterval: 500, maxPiecesOnBoard: 10, badPieceProbability: 1);

        public static GameRules GetAlwaysValidPieceRules() =>
            new GameRules(boardWidth: 8, boardHeight: 8, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5, pieceSpawnInterval: 500, maxPiecesOnBoard: 10, badPieceProbability: 0);

        public static GameMasterState GetGameMasterState(GameRules rules)
        {
            return new GameMasterState(rules);
        }
    }
}