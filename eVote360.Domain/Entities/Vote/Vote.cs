using EVote360.Domain.Base;

namespace EVote360.Domain.Entities.Vote;

public class Vote : AuditableEntity
{
    public Guid ElectionId { get; private set; }
    public Guid CitizenId { get; private set; }
    public DateTime CastAt { get; private set; } = DateTime.UtcNow;
    public string ReceiptHash { get; private set; } // comprobante (no revela contenido)

    private readonly List<VoteItem> _items = new();
    public IReadOnlyCollection<VoteItem> Items => _items.AsReadOnly();

    private Vote() { }

    public Vote(Guid electionId, Guid citizenId, string receiptHash)
    {
        if (electionId == Guid.Empty || citizenId == Guid.Empty)
            throw new ArgumentException("ElectionId/CitizenId inválido.");
        if (string.IsNullOrWhiteSpace(receiptHash))
            throw new ArgumentException("ReceiptHash requerido.", nameof(receiptHash));

        ElectionId = electionId;
        CitizenId = citizenId;
        ReceiptHash = receiptHash.Trim();
    }

    public void AddItem(Guid electionBallotId, Guid ballotOptionId)
    {
        if (electionBallotId == Guid.Empty || ballotOptionId == Guid.Empty)
            throw new ArgumentException("Ids inválidos para VoteItem.");
        _items.Add(new VoteItem(electionBallotId, ballotOptionId));
        Touch();
    }
}
