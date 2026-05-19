using Receptserver.Core.Entities;

namespace Receptserver.Core.Services;

public record OrdinationInput(string Laegemiddel, string Dosis, int AntalUdleveringer);

public interface IRecipeService
{
    Task<Recept> OpretReceptAsync(
        string ydernummer,
        string cprNummer,
        IEnumerable<OrdinationInput> ordinationer,
        int? tilknyttetApotekId = null);

    Task<IReadOnlyList<Recept>> FindAktiveRecepterForCprAsync(string cprNummer);

    Task<IReadOnlyList<Recept>> FindAktiveRecepterForApotekAsync(int apotekId);

    Task<Ordination> UdleverAsync(int ordinationId);
}