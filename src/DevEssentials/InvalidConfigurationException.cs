using System;

namespace Essentials
{
    /// <summary>
    /// exception indicating the application is configured incorrectly
    /// </summary>
    public class InvalidConfigurationException : ApplicationException
    {
        public InvalidConfigurationException()
        {
        }

        public InvalidConfigurationException(string message) : base(message)
        {
        }

        public InvalidConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
