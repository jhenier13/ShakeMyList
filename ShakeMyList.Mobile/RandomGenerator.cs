using System;

namespace ShakeMyList.Mobile
{
    public static class RandomGenerator
    {
        private static Random __generator;

        static RandomGenerator()
        {
            __generator = new Random();
        }

        public static int GenerateInteger(int min, int max)
        {
            int randomInteger = __generator.Next(min, max); 
            return randomInteger;
        }
    }
}

