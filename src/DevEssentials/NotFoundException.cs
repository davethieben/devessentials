using System;

namespace Essentials
{
    /// <summary>
    /// generic exception for cases when a model is not found for a parameter
    /// </summary>
    public class NotFoundException : ApplicationException
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
