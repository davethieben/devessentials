using System;
using System.Security.Cryptography;

namespace Essentials.Helpers
{
    public static class Randomizer
    {
        private static readonly RNGCryptoServiceProvider _randomProvider = new RNGCryptoServiceProvider();

        public static int GetInt32(int lower = 0, int upper = Int32.MaxValue)
        {
            int value;
            do
            {
                value = BitConverter.ToInt32(GetBytes(4), 0);

            } while (value < lower || value > upper);

            return value;
        }

        public static byte[] GetBytes(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var byteArray = new byte[length];
            _randomProvider.GetBytes(byteArray);
            return byteArray;
        }


    }
}
