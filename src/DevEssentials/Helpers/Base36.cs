using System;
using System.Collections.Generic;
using System.Numerics;

namespace Essentials
{
    public static class Base36
    {
        private const string _alphabet = "0123456789abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// https://codereview.stackexchange.com/a/14101
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="bigEndian"></param>
        /// <returns></returns>
        public static string Encode(byte[] bytes)
        {
            int estLength = (int)Math.Ceiling(bytes.Length * 8 / Math.Log(36, 2));

            var dividend = new BigInteger(bytes);
            var builder = new List<char>(estLength);
            while (dividend != 0)
            {
                BigInteger remainder;
                dividend = BigInteger.DivRem(dividend, 36, out remainder);
                int index = Math.Abs((int)remainder);
                builder.Add(_alphabet[index]);
            }
            builder.Reverse();
            return new string(builder.ToArray());
        }

        public static byte[] Decode(string input)
        {
            BigInteger product = new BigInteger(0);
            foreach (char c in input)
            {
                int index = _alphabet.IndexOf(c);
                product = BigInteger.Add(product, BigInteger.Multiply(index, 36));
            }
            return product.ToByteArray();
        }


    }
}
