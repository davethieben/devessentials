using System;
using System.Security.Cryptography;

namespace Essentials
{
    public static class Randomizer
    {
        private static readonly RNGCryptoServiceProvider _randomProvider = new RNGCryptoServiceProvider();

        /// <summary>
        /// returns a random integer between lower and upper
        /// </summary>
        public static int GetInt32(int lower = 0, int upper = int.MaxValue)
        {
            double seed = GetDouble();
            int spread = upper - lower;
            int value = (int)(seed * spread);

            return lower + value;
        }

        /// <summary>
        /// returns a random floating point value between 0.0 and 1.0
        /// </summary>
        public static double GetDouble()
        {
            int value = Math.Abs(BitConverter.ToInt32(GetBytes(4), 0));
            return value / (double)int.MaxValue;
        }

        public static byte[] GetBytes(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var byteArray = new byte[length];
            _randomProvider.GetNonZeroBytes(byteArray);
            return byteArray;
        }

    }
}
