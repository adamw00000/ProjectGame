using System;

namespace GameLib
{
    public class Piece: ICloneable
    {
        public bool IsValid { get; }

        private Piece(bool valid)
        {
            IsValid = valid;
        }

        public Piece(double validPieceProbability)
        {
            Random random = RandomGenerator.GetGenerator();

            IsValid = random.NextDouble() < validPieceProbability;
        }

        public object Clone()
        {
            return new Piece(IsValid);
        }
    }
}