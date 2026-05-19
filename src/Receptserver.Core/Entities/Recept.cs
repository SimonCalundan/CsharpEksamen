namespace Receptserver.Core.Entities;

public class Recept
{
    public int Id { get; set; }
    public string Ydernummer { get; set; } = string.Empty;
    public string CprNummer { get; set; } = string.Empty;
    public DateTime Oprettelsesdato { get; set; } = DateTime.UtcNow;
    public bool Lukket { get; set; }

    public int? TilknyttetApotekId { get; set; }
    public Apotek? TilknyttetApotek { get; set; }

    public List<Ordination> Ordinationer { get; set; } = new();
}