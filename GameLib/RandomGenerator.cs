using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class RandomGenerator
    {
        private static Random random;

        public static void Initialize()
        {
            random = new Random();
        }

        public static Random GetGenerator() 
        {
            return random;
        }

    }
}
