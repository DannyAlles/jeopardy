using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    [Serializable]
    public class ProfessorNotFoundException : Exception
    {
        public ProfessorNotFoundException()
        {
        }

        public ProfessorNotFoundException(string? message) : base(message)
        {
        }

        public ProfessorNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ProfessorNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
