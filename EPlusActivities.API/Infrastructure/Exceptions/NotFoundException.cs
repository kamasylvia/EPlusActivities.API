using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlusActivities.API.Infrastructure.Exceptions
{
    public class NotFoundException : CustomBaseException
    {
        public NotFoundException() { }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
