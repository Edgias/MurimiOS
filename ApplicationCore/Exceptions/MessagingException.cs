using System;

namespace Edgias.MurimiOS.Domain.Exceptions
{
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
}
