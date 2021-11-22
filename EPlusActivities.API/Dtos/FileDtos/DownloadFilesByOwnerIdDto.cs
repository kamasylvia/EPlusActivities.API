using System;

namespace EPlusActivities.API.Dtos.FileDtos
{
    public record DownloadFilesByOwnerIdDto
    {
        public Guid? Id { get; set; }

        public Guid? OwnerId { get; set; }

        public string Key { get; set; }

        public string ContentType { get; set; }
    }
}
