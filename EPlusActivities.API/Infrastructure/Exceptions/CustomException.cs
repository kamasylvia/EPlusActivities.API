using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlusActivities.API.Infrastructure.Exceptions
{
    public abstract class CustomBaseException : Exception
    {
        public CustomBaseException() { }

        public CustomBaseException(string message) : base(message) { }

        public CustomBaseException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
