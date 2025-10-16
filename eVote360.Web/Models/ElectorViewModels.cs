namespace EVote360.Web.Models
{
    public class ElectorIdVm
    {
        public string? NationalId { get; set; }
    }

    public class BoletaVm
    {
        public Guid ElectionId { get; set; }
        public Guid CitizenId { get; set; }
        public List<PuestoVm> Puestos { get; set; } = new();
    }
    public class PuestoVm
    {
        public Guid PositionId { get; set; }
        public string PositionName { get; set; } = "";
        public bool YaVotado { get; set; }
        public int Partidos { get; set; }
        public int Candidatos { get; set; }
    }

    public class OpcionVm
    {
        public Guid? CandidateId { get; set; }
        public string Display { get; set; } = "";
    }

    public class OpcionesVm
    {
        public Guid ElectionId { get; set; }
        public Guid CitizenId { get; set; }
        public Guid PositionId { get; set; }
        public Guid? CandidateId { get; set; }
        public List<OpcionVm> Opciones { get; set; } = new();
    }
}
