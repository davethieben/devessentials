using System;
using System.IO;
using System.Security.Cryptography;

namespace Essentials.Helpers
{
    public interface IEncrypter
    {
        string DecryptString(string cipherText, string sharedSecret);
        string EncryptString(string plainText, string sharedSecret);
    }

    public class AesEncrypter : IEncrypter
    {
        private readonly byte[] _salt;

        public AesEncrypter(byte[] salt)
        {
            Contract.Requires(salt, nameof(salt));
            _salt = salt;
        }

        public static byte[] NewSalt() => Guid.NewGuid().ToByteArray();

        /// <summary>
        /// Encrypt the given string using AES. The string can be decrypted using 
        /// DecryptString(). The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        public string EncryptString(string plainText, string sharedSecret)
        {
            Contract.Requires(plainText, nameof(plainText));
            Contract.Requires(sharedSecret, nameof(sharedSecret));

            Aes aes = Aes.Create();
            string outStr;

            try
            {
                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                aes.Key = key.GetBytes(aes.KeySize / 8);

                // create a encryptor to perform the stream transform.
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // create the streams used for encryption.
                using (var memory = new MemoryStream())
                {
                    // prepend the IV
                    memory.Write(BitConverter.GetBytes(aes.IV.Length), 0, sizeof(int));
                    memory.Write(aes.IV, 0, aes.IV.Length);

                    using (var encrypted = new CryptoStream(memory, encryptor, CryptoStreamMode.Write))
                    using (var encryptedWriter = new StreamWriter(encrypted))
                    {
                        //Write all data to the stream.
                        encryptedWriter.Write(plainText);
                    }
                    outStr = Convert.ToBase64String(memory.ToArray());
                }
            }
            finally
            {
                aes.Clear();
            }

            return outStr;
        }

        /// <summary>
        /// Decrypt the given string. Assumes the string was encrypted using 
        /// EncryptString(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        public string DecryptString(string cipherText, string sharedSecret)
        {
            Contract.Requires(cipherText, nameof(cipherText));
            Contract.Requires(sharedSecret, nameof(sharedSecret));

            Aes aes = Aes.Create();
            string plaintext;

            try
            {
                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);

                using (var memory = new MemoryStream(bytes))
                {
                    aes.Key = key.GetBytes(aes.KeySize / 8);

                    // Get the initialization vector from the encrypted stream
                    aes.IV = ReadByteArray(memory);

                    // Create a decrytor to perform the stream transform.
                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var decryptStream = new CryptoStream(memory, decryptor, CryptoStreamMode.Read))
                    using (var decryptReader = new StreamReader(decryptStream))
                    {
                        plaintext = decryptReader.ReadToEnd();
                    }
                }
            }
            finally
            {
                aes.Clear();
            }

            return plaintext;
        }

        private static byte[] ReadByteArray(Stream stream)
        {
            // read size out of first 4 bytes:
            var rawLength = new byte[sizeof(int)];
            if (stream.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            // then get the rest determined by the size:
            var buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }

    }
}
