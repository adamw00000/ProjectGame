using ConnectionLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class GameMaster
    {
        private readonly IConnection connection;

        private readonly GameMasterBoard board;
        private readonly GameRules rules;

        public GameMaster()
        {

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
    }
}