using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class Photo
    {
        [Required]
        public Guid? Id { get; set; }

        public byte[] Content { get; set; }
    }
}
