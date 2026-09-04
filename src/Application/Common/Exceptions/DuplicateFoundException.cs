using System;

namespace EdgIQ.MurimiOS.Application.Common.Exceptions;
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
