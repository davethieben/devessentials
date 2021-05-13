using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Essentials
{
    public static class ZipHelper
    {
        private const int ZIP_LEAD_BYTES = 0x04034b50;

        /// <summary>
        /// determines if the data in the buffer is a zip file
        /// </summary>
        public static bool IsPkZipCompressedData(byte[] data)
        {
            if (data == null || data.Length < 4)
                return false;

            // if the first 4 bytes of the array are the ZIP signature then it is compressed data
            return (BitConverter.ToInt32(data, 0) == ZIP_LEAD_BYTES);
        }

        /// <summary>
        /// determines if the data in the buffer is a zip file
        /// </summary>
        public static bool IsPkZipCompressedData(Stream stream)
        {
            byte[] data = new byte[4];
            stream.Read(data, 0, 4);
            stream.Seek(0, SeekOrigin.Begin);

            return IsPkZipCompressedData(data);
        }

        public static Task ExtractToFileAsync(this ZipArchiveEntry source, string destinationFileName, bool overwrite)
        {
            return Task.Run(() =>
            {
                source.ExtractToFile(destinationFileName, overwrite);
            });
        }

    }
}