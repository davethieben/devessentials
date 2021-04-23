using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Essentials.IO
{
    public static class IOExtensions
    {

        /// <summary>
        /// read the given number of bytes from the stream
        /// </summary>
        public static async Task<byte[]> ReadBytesAsync(this Stream input, long numBytes)
        {
            byte[] data = new byte[numBytes];
            await input.ReadAsync(data, 0, (int)numBytes);
            return data;
        }

        /// <summary>
        /// read all the bytes from the given stream into a buffer and return as an array.
        /// </summary>
        public static async Task<byte[]> ReadAllBytesAsync(this Stream stream, int bufferSize = 65536)
        {
            var output = new List<byte>();
            byte[] data = new byte[bufferSize];
            int bytesRead = 0;

            do
            {
                bytesRead = await stream.ReadAsync(data, 0, bufferSize);
                output.AddRange(data.Take(bytesRead));
            }
            while (bytesRead > 0);

            return output.ToArray();
        }

        public static string? FindNearestWith(this DirectoryInfo directory, string searchPattern)
        {
            directory.IsRequired();
            Contracts.Require(searchPattern, nameof(searchPattern));

            // the caller may have sent in a file name
            if (File.Exists(directory.FullName))
                directory = directory.Parent;

            var current = directory;
            while (current != null)
            {
                if (current.Exists && current.GetFiles(searchPattern).Any()
                    || current.Exists && current.GetDirectories(searchPattern).Any())
                    return current.FullName;

                current = current.Parent;
            }

            return null;
        }

        public static string? FindNearestNamed(this DirectoryInfo directory, string name)
        {
            directory.IsRequired();
            Contracts.Require(name, nameof(name));

            var current = directory;
            while (current != null)
            {
                if (current.Name.IsEquivalent(name))
                    return current.FullName;

                current = current.Parent;
            }

            return null;
        }


    }
}
