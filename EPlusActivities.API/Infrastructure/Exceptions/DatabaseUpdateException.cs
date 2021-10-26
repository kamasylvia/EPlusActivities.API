using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlusActivities.API.Infrastructure.Exceptions
{
    public class DatabaseUpdateException : CustomBaseException
    {
        public DatabaseUpdateException() { }

        public DatabaseUpdateException(string message) : base(message) { }

        public DatabaseUpdateException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
