using Receptserver.Core.Entities;

namespace Receptserver.Core.Dtos;

public static class Mappers
{
    public static ApotekDto ToDto(this Apotek a) => new(a.Id, a.Navn, a.Adresse);

    public static LaegehusDto ToDto(this Laegehus l) => new(l.Id, l.Ydernummer, l.Navn, l.Adresse);

    public static OrdinationDto ToDto(this Ordination o) =>
        new(o.Id, o.Laegemiddel, o.Dosis, o.AntalUdleveringer, o.AntalForetagneUdleveringer);

    public static ReceptDto ToDto(this Recept r) =>
        new(
            r.Id,
            r.Ydernummer,
            r.CprNummer,
            r.Oprettelsesdato,
            r.Lukket,
            r.TilknyttetApotek?.ToDto(),
            r.Ordinationer.Select(o => o.ToDto()).ToList());
}
