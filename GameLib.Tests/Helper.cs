using System;

namespace GameLib.Tests
{
    internal static class Helper
    {
        public static GameRules GetDefaultRules(int teamsize = 5) =>
            new GameRules(boardWidth: 8, boardHeight: 8, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5, teamSize: teamsize);

        public static GameRules GetOddSizeBoardRules(int teamsize = 5) =>
            new GameRules(boardWidth: 7, boardHeight: 7, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5, teamSize: teamsize); //odd-sized board

        public static GameRules GetStaticDefaultRules(int teamsize = 5) =>
            new GameRules(boardWidth: 8, boardHeight: 8, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5, teamSize: teamsize); //Nie ruszac

        public static GameRules GetAlwaysInvalidPieceRules(int teamsize = 5) =>
           new GameRules(boardWidth: 8, boardHeight: 8, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5, teamSize: teamsize, pieceSpawnInterval: 500, maxPiecesOnBoard: 10, badPieceProbability: 1);

        public static GameRules GetAlwaysValidPieceRules(int teamsize = 5) =>
            new GameRules(boardWidth: 8, boardHeight: 8, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5, teamSize: teamsize, pieceSpawnInterval: 500, maxPiecesOnBoard: 10, badPieceProbability: 0);

        public static GameRules GetAlwaysInvalidPieceRules() =>
           new GameRules(boardWidth: 9, boardHeight: 8, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5, pieceSpawnInterval: 500, maxPiecesOnBoard: 10, badPieceProbability: 1);
        public static GameRules GetRulesWithSmallTeamSize() =>
            new GameRules(teamSize: 1);

        public static GameRules GetAlwaysValidPieceRules() =>
            new GameRules(boardWidth: 8, boardHeight: 10, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5, pieceSpawnInterval: 500, maxPiecesOnBoard: 10, badPieceProbability: 0);
        public static GameRules GetRulesWithMediumTeamSize() =>
            new GameRules(teamSize: 3);

        public static GameMasterState GetGameMasterState(GameRules rules)
        {
            return new GameMasterState(rules);
        }

        public static void AddPlayers(GameMasterState state, GameRules rules)
        {
            for (int i = 0; i < rules.TeamSize; ++i)
            {
                state.JoinGame(i, 0, false);
                state.JoinGame(i + rules.TeamSize, 1, false);
            }
        }
    }
}