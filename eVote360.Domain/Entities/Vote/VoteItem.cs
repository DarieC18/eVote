using EVote360.Domain.Base;

namespace EVote360.Domain.Entities.Vote;

public class VoteItem : AuditableEntity
{
    public Guid VoteId { get; private set; }
    public Guid ElectionBallotId { get; private set; }
    public Guid BallotOptionId { get; private set; }

    private VoteItem() { }

    public VoteItem(Guid electionBallotId, Guid ballotOptionId)
    {
        if (electionBallotId == Guid.Empty || ballotOptionId == Guid.Empty)
            throw new ArgumentException("Ids inválidos para VoteItem.");
        ElectionBallotId = electionBallotId;
        BallotOptionId = ballotOptionId;
    }
}

