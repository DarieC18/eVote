namespace EVote360.Application.DTOs.Response
{
    public class VoteResponseDto
    {
        public Guid VoteId { get; set; }
        public Guid ElectionId { get; set; }
        public Guid CitizenId { get; set; }
        public DateTime CastAt { get; set; }
        public string ReceiptHash { get; set; }
        public List<VoteItemDto> Items { get; set; } = new List<VoteItemDto>();
    }

    public class VoteItemDto
    {
        public Guid ElectionBallotId { get; set; }
        public Guid BallotOptionId { get; set; }
    }
}
