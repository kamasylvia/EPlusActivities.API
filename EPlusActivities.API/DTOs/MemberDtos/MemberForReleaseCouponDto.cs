namespace EPlusActivities.API.DTOs.MemberDtos
{
    public class MemberForReleaseCouponRequestDto
    {
        public string couponActiveCode { get; set; }
        public string memberId { get; set; }
        public int qty { get; set; }
        public string reason { get; set; }
    }
    public class MemberForReleaseCouponResponseDto
    {
        public HeaderDto Header { get; set; }
        public BodyForReleaseCouponDto Body { get; set; }
    }

    public class BodyForReleaseCouponDto
    {
        public ContentForReleaseCouponDto Content { get; set; }
    }

    public class ContentForReleaseCouponDto
    {
        public string HideCouponCode { get; set; }
    }
}
