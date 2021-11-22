using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.MemberDtos
{
    public record MemberForUpdateCreditRequestDto
    {
        public string memberId { get; set; }

        public int points { get; set; }

        public string reason { get; set; }

        public string sheetId { get; set; }

        public CreditUpdateType updateType { get; set; }
    }

    public record MemberForUpdateCreditResponseDto
    {
        public HeaderDto Header { get; set; }

        public BodyForUpdateCreditDto Body { get; set; }
    }

    public record BodyForUpdateCreditDto
    {
        public ContentForUpdateCreditDto Content { get; set; }
    }

    public record ContentForUpdateCreditDto
    {
        public string MemberId { get; set; }

        public int OldPoints { get; set; }

        public int NewPoints { get; set; }

        public string RecordId { get; set; }
    }
}
