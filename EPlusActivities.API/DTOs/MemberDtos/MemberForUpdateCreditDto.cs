using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.DTOs.MemberDtos
{
    public class MemberForUpdateCreditRequestDto
    {
        public string memberId { get; set; }

        public int points { get; set; }

        public string reason { get; set; }

        [StringLength(12, MinimumLength = 12, ErrorMessage = "交易流水必须为 10 位数")]
        public string sheetId { get; set; }

        public CreditUpdateType updateType { get; set; }
    }

    public class MemberForUpdateCreditResponseDto
    {
        public HeaderDto Header { get; set; }

        public BodyForUpdateCreditDto Body { get; set; }
    }

    public class BodyForUpdateCreditDto
    {
        public ContentForUpdateCreditDto Content { get; set; }
    }

    public class ContentForUpdateCreditDto
    {
        public string MemberId { get; set; }

        public int OldPoints { get; set; }

        public int NewPoints { get; set; }

        public string RecordId { get; set; }
    }
}
