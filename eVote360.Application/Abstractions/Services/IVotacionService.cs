using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace eVote360.Application.Abstractions.Services
{
    public interface IVotacionService
    {
        Task<(bool ok, string error)> ValidarElegibilidadAsync(string nationalId, CancellationToken ct = default);
        Task<(bool ok, string error, Guid electionId)> ObtenerEleccionActivaAsync(CancellationToken ct = default);
        Task<(bool ok, string error)> RegistrarVotoAsync(Guid electionId, Guid citizenId, Guid positionId, Guid? candidateId, CancellationToken ct = default);
        Task<bool> CiudadanoCompletoVotosAsync(Guid electionId, Guid citizenId, CancellationToken ct = default);
        Task<bool> YaEjecutoVotoAsync(Guid electionId, Guid citizenId, CancellationToken ct = default);
        Task<IReadOnlyList<(Guid positionId, string positionName, int partidos, int candidatos)>> ConteoPorPuestoAsync(Guid electionId, CancellationToken ct = default);
        Task<IReadOnlyList<(Guid positionId, string positionName)>> PuestosFaltantesAsync(Guid electionId, Guid citizenId, CancellationToken ct = default);


        Task<(string ciudadanoNombre, string ciudadanoEmail, IReadOnlyList<(string puesto, string opcion)> detalle)>
            ResumenVotoAsync(Guid electionId, Guid citizenId, CancellationToken ct = default);
    }
}
