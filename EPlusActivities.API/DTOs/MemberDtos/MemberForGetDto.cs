namespace EPlusActivities.API.DTOs.MemberDtos
{
    public class MemberForGetDto
    {
        public HeaderDto Header { get; set; }

        public BodyForGetDto Body { get; set; }
    }

    public class BodyForGetDto
    {
        public ContentForGetDto Content { get; set; }
    }

    public class ContentForGetDto
    {
        public string MemberId { get; set; }

        public string Mobile { get; set; }

        public string NickName { get; set; }

        public string PhotoUrl { get; set; }

        public int Points { get; set; }
    }
}
