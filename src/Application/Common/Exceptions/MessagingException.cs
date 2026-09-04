using System;

namespace EdgIQ.MurimiOS.Application.Common.Exceptions;
    public class MessagingException : Exception
    {
        public MessagingException()
        {

        }

        public MessagingException(string message)
            : base(message)
        {

        }

        public MessagingException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
