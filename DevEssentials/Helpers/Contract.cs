using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Essentials
{
    public static class Contract
    {
        [return: NotNull]
        public static T IsRequired<T>([NotNull] this T? input, string? paramName = null)
        {
            if (input == null)
                throw new ContractException(paramName ?? typeof(T).FullName);

            return input;
        }

        [return: NotNull]
        public static string Requires(string? input, string paramName)
        {
            if (string.IsNullOrEmpty(input))
                throw new ContractException(paramName);

            return input;
        }

        [return: NotNull]
        public static int Requires(int? input, string paramName)
        {
            if (input == null || input == 0)
                throw new ContractException(paramName);

            return input.Value;
        }

        [return: NotNull]
        public static IEnumerable<T> Requires<T>(IEnumerable<T>? input, string paramName)
        {
            if (input.IsNullOrEmpty())
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
