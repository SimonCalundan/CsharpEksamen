namespace Receptserver.Core.Dtos;

public record ApotekDto(int Id, string Navn, string Adresse);

public record LaegehusDto(int Id, string Ydernummer, string Navn, string Adresse);

public record OrdinationDto(
    int Id,
    string Laegemiddel,
    string Dosis,
    int AntalUdleveringer,
    int AntalForetagneUdleveringer);

public record ReceptDto(
    int Id,
    string Ydernummer,
    string CprNummer,
    DateTime Oprettelsesdato,
    bool Lukket,
    ApotekDto? TilknyttetApotek,
    IReadOnlyList<OrdinationDto> Ordinationer);

public record OpretOrdinationRequest(string Laegemiddel, string Dosis, int AntalUdleveringer);

public record OpretReceptRequest(
    string Ydernummer,
    string CprNummer,
    int? TilknyttetApotekId,
    IReadOnlyList<OpretOrdinationRequest> Ordinationer);

public record ErrorResponse(string Error, string? Field = null);
