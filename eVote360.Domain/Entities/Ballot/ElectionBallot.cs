namespace EVote360.Domain.Entities.Ballot;

public class ElectionBallot
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ElectionId { get; private set; }
    public Guid PositionId { get; private set; }

    public ElectionBallot(Guid electionId, Guid positionId)
    {
        ElectionId = electionId;
        PositionId = positionId;
    }
}
