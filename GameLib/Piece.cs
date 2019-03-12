using System;

namespace GameLib
{
    public class Piece
    {
        public bool IsValid { get; }

        public Piece(double validPieceProbability)
        {
            Random random = RandomGenerator.GetGenerator();

            IsValid = random.NextDouble() < validPieceProbability;
        }
    }
}