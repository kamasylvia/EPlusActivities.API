using System;

namespace FileService.Infrastructure.Exceptions
{
    public class NotFoundException : CustomBaseException
    {
        public NotFoundException() { }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
