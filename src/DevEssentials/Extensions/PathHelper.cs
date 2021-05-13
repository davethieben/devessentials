using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Essentials
{
    public static class PathHelper
    {
        public static string ApplicationPath => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// get the path to the parent folder of the given path
        /// </summary>
        public static string GetParent(string path) => new DirectoryInfo(path).Parent.FullName;

        /// <summary>
        /// if the given path does not exist, creates all the folders needed in the path
        /// </summary>
        /// <param name="path"></param>
        public static void EnsureCreated(string path)
        {
            if (!Directory.Exists(path))
            {
                EnsureCreated(GetParent(path));
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// attempts to remove the given path, and if it throws an IOException, retries number of times.
        /// the final attempt will rethrow the exception, if there is one thrown
        /// </summary>
        public static void RemoveDirectoryWithRetry(string path, int retries = 3)
        {
            if (!Directory.Exists(path))
                return;

            // sometimes Directory.Delete() breaks because of file locks so retry it a couple times
            for (int i = 0; i < retries - 1; i++)
            {
                try
                {
                    Directory.Delete(path, recursive: true);
                    return;
                }
                catch (IOException)
                {
                    Thread.Sleep((i + 1) * 50);
                }
            }

            // one final attempt to bubble up the exception
            Directory.Delete(path, recursive: true);
        }

        /// <summary>
        /// delays # of milliseconds before attempting to remove the given path
        /// </summary>
        public static void RemoveDirectoryWithDelay(string path, int millisecondsDelay = 1000)
        {
            Task.Delay(millisecondsDelay)
                .ContinueWith(t =>
                {
                    Directory.Delete(path, recursive: true);
                });
        }

        public static DirectoryInfo AssemblyPathFor<T>() =>
            new DirectoryInfo(Path.GetDirectoryName(typeof(T).Assembly.Location));

    }
}
