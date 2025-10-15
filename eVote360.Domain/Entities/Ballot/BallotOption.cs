using EVote360.Domain.Base;

namespace EVote360.Domain.Entities.Ballot;


/// opcion de boleta para un puesto específico en la elección (incluye "Ninguno").
/// candidateId y PartyId pueden ser nulos para representar "Ninguno".

public class BallotOption : AuditableEntity
{
    public Guid ElectionBallotId { get; private set; }

    public Guid? CandidateId { get; private set; }
    public Guid? PartyId { get; private set; }

    public bool IsNinguno { get; private set; }

    private BallotOption() { }

    public static BallotOption CreateCandidateOption(Guid electionBallotId, Guid candidateId, Guid partyId)
    {
        if (electionBallotId == Guid.Empty || candidateId == Guid.Empty || partyId == Guid.Empty)
            throw new ArgumentException("Ids inválidos para opción de candidato.");

        return new BallotOption
        {
            ElectionBallotId = electionBallotId,
            CandidateId = candidateId,
            PartyId = partyId,
            IsNinguno = false
        };
    }

    public static BallotOption CreateNinguno(Guid electionBallotId)
    {
        if (electionBallotId == Guid.Empty)
            throw new ArgumentException("ElectionBallotId inválido.");

        return new BallotOption
        {
            ElectionBallotId = electionBallotId,
            CandidateId = null,
            PartyId = null,
            IsNinguno = true
        };
    }
}

