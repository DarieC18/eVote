using EVote360.Domain.Base;

namespace EVote360.Domain.Entities.Ballot;

public class ElectionBallot : AuditableEntity
{
    public Guid ElectionId { get; private set; }
    public Guid PositionId { get; private set; }

    private ElectionBallot() { }

    public ElectionBallot(Guid electionId, Guid positionId)
    {
        if (electionId == Guid.Empty || positionId == Guid.Empty)
            throw new ArgumentException("ElectionId/PositionId inválidos.");
        ElectionId = electionId;
        PositionId = positionId;
    }
}

