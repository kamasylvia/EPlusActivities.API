using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Exceptions
{
    public class RemoteServiceException : CustomBaseException
    {
        public RemoteServiceException() { }

        public RemoteServiceException(string message) : base(message) { }

        public RemoteServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
