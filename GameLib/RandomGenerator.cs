using System;

namespace GameLib
{
    public static class RandomGenerator
    {
        private static readonly Random random = new Random();

        public static Random GetGenerator()
        {
            return random;
        }
    }
}