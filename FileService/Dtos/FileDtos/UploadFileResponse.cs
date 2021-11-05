using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.Dtos.FileDtos
{
    public record UploadFileResponse
    {
        public bool Succeeded { get; set; }
    }
}
