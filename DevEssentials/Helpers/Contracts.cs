using System;
using System.Diagnostics.CodeAnalysis;

namespace Essentials
{
    public static class Contracts
    {
        [return: NotNull]
        public static T IsRequired<T>(this T input)
        {
            if (input == null)
                throw new ContractException(typeof(T).FullName);

            return input;
        }

        [return: NotNull]
        public static string Require(string input, string paramName)
        {
            if (string.IsNullOrEmpty(input))
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
