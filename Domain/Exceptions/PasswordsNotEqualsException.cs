using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    [Serializable]
    public class PasswordsNotEqualsException : Exception
    {
        public PasswordsNotEqualsException()
        {
        }

        public PasswordsNotEqualsException(string? message) : base(message)
        {
        }

        public PasswordsNotEqualsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PasswordsNotEqualsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
