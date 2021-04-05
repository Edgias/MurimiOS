using System;

namespace Murimi.ApplicationCore.Exceptions
{
    public class DuplicateFoundException : Exception
    {
        public DuplicateFoundException()
        {

        }

        public DuplicateFoundException(string message)
            : base(message)
        {

        }

        public DuplicateFoundException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
