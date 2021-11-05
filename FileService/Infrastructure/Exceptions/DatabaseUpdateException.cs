using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Exceptions
{
    public class DatabaseUpdateException : CustomBaseException
    {
        public DatabaseUpdateException() { }

        public DatabaseUpdateException(string message) : base(message) { }

        public DatabaseUpdateException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
