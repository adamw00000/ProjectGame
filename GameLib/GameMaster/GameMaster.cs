using ConnectionLib;
using System;
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
                await Task.Delay(rules.PieceSpawnInterval).ConfigureAwait(false);
            }
        }

        public void MoveAgent(int agentId, MoveDirection moveDirection)
        {
            throw new NotImplementedException();
        }

        public void PickPiece(int agentId)
        {
            throw new NotImplementedException();
        }

        public void PutPiece(int agentId)
        {
            throw new NotImplementedException();
        }

        public void Discover(int agentId)
        {
            throw new NotImplementedException();
        }

        public void CheckPiece(int agentId)
        {
            throw new NotImplementedException();
        }

        public void DestroyPiece(int agentId)
        {
            throw new NotImplementedException();
        }

        public void CommunicationRequestWithData(int agentId, Object data)
        {
            throw new NotImplementedException();
        }

        public void CommunicationAgreementWithData(int agentId, Object data)
        {
            throw new NotImplementedException();
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
}