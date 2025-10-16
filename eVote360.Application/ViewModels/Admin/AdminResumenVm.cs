namespace EVote360.Application.ViewModels.Admin;

public class AdminResumenVm
{
    public int? SelectedYear { get; set; }
    public List<int> Years { get; set; } = new();
    public List<EleccionResumenVm> Elecciones { get; set; } = new();
}

public class EleccionResumenVm
{
    public Guid ElectionId { get; set; }
    public string Nombre { get; set; } = "";
    public DateTime Fecha { get; set; }
    public int CantidadPartidos { get; set; }
    public int CantidadCandidatos { get; set; }
    public int TotalVotosEmitidos { get; set; }
}
