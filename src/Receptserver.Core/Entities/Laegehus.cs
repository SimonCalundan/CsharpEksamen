namespace Receptserver.Core.Entities;

public class Laegehus
{
    public int Id { get; set; }
    public string Ydernummer { get; set; } = string.Empty;
    public string Navn { get; set; } = string.Empty;
    public string Adresse { get; set; } = string.Empty;
}