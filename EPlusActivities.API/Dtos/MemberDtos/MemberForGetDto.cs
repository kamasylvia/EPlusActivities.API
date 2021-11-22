namespace EPlusActivities.API.Dtos.MemberDtos
{
    public record MemberForGetDto
    {
        public HeaderDto Header { get; set; }

        public BodyForGetDto Body { get; set; }
    }

    public record BodyForGetDto
    {
        public ContentForGetDto Content { get; set; }
    }

    public record ContentForGetDto
    {
        public string MemberId { get; set; }

        public string Mobile { get; set; }

        public string NickName { get; set; }

        public string PhotoUrl { get; set; }

        public int Points { get; set; }
    }
}
