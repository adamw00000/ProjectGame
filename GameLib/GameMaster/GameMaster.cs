using ConnectionLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameLib
{
    public class GameMaster
    {
        private readonly IConnection connection;

        private GameMasterState state;
        private readonly GameRules rules;

        public GameMaster()
        {

        }

        public void GenerateBoard()
        {
            state = new GameMasterState(rules);
        }
        
        public async Task GeneratePieces()
        {
            while (!state.GameEnded)
            {
                state.GeneratePiece();
                await Task.Delay(rules.PieceSpawnInterval);
            }
        }

        public async Task StartGame()
        {
            var generatePiecesTask = GeneratePieces();

            while (!state.GameEnded)
            {
                //game
            }

            await generatePiecesTask;
        }
    }

    public enum Team
    {
        Blue = 0,
        Red = 1
    }
}