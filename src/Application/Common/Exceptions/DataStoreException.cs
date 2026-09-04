using System;

namespace EdgIQ.MurimiOS.Application.Common.Exceptions;
    public class DataStoreException : Exception
    {
        public DataStoreException()
        {

        }

        public DataStoreException(string message)
            : base(message)
        {

        }

        public DataStoreException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
