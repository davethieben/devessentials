using System;
using System.Threading.Tasks;

namespace Essentials
{
    public static class Tasks
    {
        public static void Noop<T>(T t) { }
        public static Task<bool> False => Task.FromResult(false);
        public static Task<int> Zero => Task.FromResult(0);
        public static Task<string> EmptyString => Task.FromResult(string.Empty);

        public static Task Catch(this Task task, Action<Exception> handler)
        {
            return task.ContinueWith(
                t => handler(t.Exception.InnerException),
                TaskContinuationOptions.OnlyOnFaulted);
        }

        public static async Task SwallowIfCancelled(this Task task)
        {
            if (task == null)
                throw new ContractException(nameof(task));

            try
            {
                await task;
            }
            catch (TaskCanceledException) { }
        }

        /// <summary>
        /// removes a warning that this Task is not "await"ed
        /// </summary>
        public static void AndForget(this Task task) =>
            task.IsRequired().ConfigureAwait(false);

    }
}
