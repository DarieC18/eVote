namespace EVote360.Domain.Entities.Ballot;

public class BallotOption
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ElectionId { get; private set; }
    public Guid PositionId { get; private set; }
    public Guid? PartyId { get; private set; }
    public Guid? CandidateId { get; private set; }
    public bool IsNinguno { get; private set; }

    private BallotOption() { }

    private BallotOption(Guid electionId, Guid positionId, Guid? partyId, Guid? candidateId, bool isNinguno)
    {
        ElectionId = electionId;
        PositionId = positionId;
        PartyId = partyId;
        CandidateId = candidateId;
        IsNinguno = isNinguno;
    }
    public static BallotOption CreateNinguno(Guid electionBallotId, Guid positionId)
        => new BallotOption(
            electionId: electionBallotId,
            positionId: positionId,
            partyId: null,
            candidateId: null,
            isNinguno: true);

    public static BallotOption ForCandidate(Guid electionId, Guid positionId, Guid partyId, Guid candidateId)
        => new BallotOption(electionId, positionId, partyId, candidateId, false);
}
