using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    [Serializable]
    public class LoginExistException : Exception
    {
        public LoginExistException()
        {
        }

        public LoginExistException(string? message) : base(message)
        {
        }

        public LoginExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected LoginExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
