using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlusActivities.API.Infrastructure.Exceptions
{
    public class BadRequestException : CustomBaseException
    {
        public BadRequestException() { }

        public BadRequestException(string message) : base(message) { }

        public BadRequestException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
