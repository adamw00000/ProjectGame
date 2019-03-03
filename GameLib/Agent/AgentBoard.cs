using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class AgentBoard
    {
        public readonly AgentField[,] Board;
        public int Width => Board.GetLength(0);
        public int Height => Board.GetLength(1);

        public AgentBoard(GameRules rules)
        {
            // konstrukcja tablicy i wypełnienie AgentField.FieldState odpowiednimi wartościami
            throw new NotImplementedException();
        }
    }
}