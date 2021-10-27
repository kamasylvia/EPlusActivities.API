using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.Dtos.FileDtos
{
    public class GetFileByOwnerIdResponseDto
    {
        public Guid? Id { get; set; }

        public Guid? OwnerId { get; set; }

        public string Key { get; set; }

        public string ContentType { get; set; }
    }
}
