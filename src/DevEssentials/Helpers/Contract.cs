using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Essentials
{
    public static class Contract
    {
#if NETSTANDARD2_1_OR_GREATER
        [return: NotNull]
#endif
        public static T IsRequired<T>(
#if NETSTANDARD2_1_OR_GREATER
        [NotNull]
#endif
            this T? input, string? paramName = null)
        {
            if (input == null)
                throw new ContractException(paramName ?? typeof(T).FullName);

            return input;
        }

        public static void Requires(bool test, string paramName)
        {
            if (!test)
                throw new ContractException(paramName);
        }

#if NETSTANDARD2_1_OR_GREATER
        [return: NotNull]
#endif
        public static string Requires(string? input, string paramName)
        {
            if (input == null || string.IsNullOrEmpty(input))
                throw new ContractException(paramName);

            return input;
        }

#if NETSTANDARD2_1_OR_GREATER
        [return: NotNull]
#endif
        public static int Requires(int? input, string paramName)
        {
            if (input == null || input == 0)
                throw new ContractException(paramName);

            return input.Value;
        }

#if NETSTANDARD2_1_OR_GREATER
        [return: NotNull]
#endif
        public static IEnumerable<T> Requires<T>(IEnumerable<T>? input, string paramName)
        {
            if (input == null || input.IsNullOrEmpty())
                throw new ContractException(paramName);

            return input;
        }




    }

    public class ContractException : ArgumentException
    {
        public ContractException()
        {
        }

        public ContractException(string paramName) : base($"'{paramName}' is required", paramName)
        {
        }

        public ContractException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ContractException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

    }

}
