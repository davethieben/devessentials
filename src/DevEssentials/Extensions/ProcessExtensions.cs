using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Essentials
{
    /// <summary>
    /// helpers for <see cref="System.Diagnostics.Process"/>
    /// </summary>
    public static class ProcessExtensions
    {
        public static string ToDisplayString(this Process process)
        {
            try
            {
                return process != null ? $"[{process.Id}] {process.ProcessName}" : "[null]";
            }
            catch
            {
                return "[invalid]";
            }
        }

        public static bool HasExitedSafe(this Process process)
        {
            try
            {
                return (process?.HasExited != false);
            }
            catch
            {
                return true;
            }
        }

        public static bool TryShutdown(this Process process, int attempts = 3, TimeSpan? totalWait = null)
        {
            int wait = (int)Math.Floor((totalWait ?? TimeSpan.FromSeconds(3)).TotalMilliseconds / attempts / 2);

            bool exited = process.HasExitedSafe();

            for (int attempt = 0; !exited && attempt < attempts; attempt++)
            {
                process.WaitForExit(wait);

                exited = process.HasExitedSafe();

                if (!exited)
                {
                    Task.Delay(wait).Wait();
                    exited = process.HasExitedSafe();
                }
            }

            return exited;
        }

        public static bool TryShutdownAndKill(this Process process, int attempts = 3, TimeSpan? totalWait = null)
        {
            int wait = (int)Math.Floor((totalWait ?? TimeSpan.FromSeconds(3)).TotalMilliseconds / attempts / 2);

            bool exited = process.TryShutdown(attempts, totalWait);

            if (!exited && !process.HasExitedSafe())
            {
                process.Kill();

                exited = process.HasExitedSafe();

                if (!exited)
                {
                    Task.Delay(wait).Wait();
                    exited = process.HasExitedSafe();
                }
            }

            return exited;
        }

    }
}
