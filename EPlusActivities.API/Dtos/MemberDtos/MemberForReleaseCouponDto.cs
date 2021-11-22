namespace EPlusActivities.API.Dtos.MemberDtos
{
    public record MemberForReleaseCouponRequestDto
    {
        public string couponActiveCode { get; set; }
        public string memberId { get; set; }
        public int qty { get; set; }
        public string reason { get; set; }
    }
    public record MemberForReleaseCouponResponseDto
    {
        public HeaderDto Header { get; set; }
        public BodyForReleaseCouponDto Body { get; set; }
    }

    public record BodyForReleaseCouponDto
    {
        public ContentForReleaseCouponDto Content { get; set; }
    }

    public record ContentForReleaseCouponDto
    {
        public string HideCouponCode { get; set; }
    }
}
