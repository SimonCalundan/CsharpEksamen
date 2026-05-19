namespace Receptserver.Core.Entities;

public class Ordination
{
    public int Id { get; set; }
    public string Laegemiddel { get; set; } = string.Empty;
    public string Dosis { get; set; } = string.Empty;
    public int AntalUdleveringer { get; set; }
    public int AntalForetagneUdleveringer { get; set; }

    public int ReceptId { get; set; }
    public Recept? Recept { get; set; }
}