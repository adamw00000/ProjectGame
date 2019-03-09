using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class Piece
    {
        public bool IsValid { get; private set; }

        public Piece(double validPieceProbability)
        {
            Random random = RandomGenerator.GetGenerator();

            IsValid = random.NextDouble() < validPieceProbability ? true : false;
        }

    }
}
