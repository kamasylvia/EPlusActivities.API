using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.Entities
{
    public class Address
    {
        [Key]
        public string Id { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string DetailedAddress { get; set; }
        public string Postcode { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}