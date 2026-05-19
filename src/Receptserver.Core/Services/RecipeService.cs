using Microsoft.EntityFrameworkCore;
using Receptserver.Core.Entities;
using Receptserver.Core.Exceptions;
using Receptserver.Core.Persistence;

namespace Receptserver.Core.Services;

public class RecipeService : IRecipeService
{
    private readonly IReceptDbContext _db;

    public RecipeService(IReceptDbContext db)
    {
        _db = db;
    }

    public async Task<Recept> OpretReceptAsync(
        string ydernummer,
        string cprNummer,
        IEnumerable<OrdinationInput> ordinationer,
        int? tilknyttetApotekId = null)
    {
        if (string.IsNullOrWhiteSpace(ydernummer))
            throw new ValidationException(nameof(ydernummer), "må ikke være tom");

        var normalizedCpr = NormalizeCpr(cprNummer);

        var ordinationList = ordinationer?.ToList() ?? new List<OrdinationInput>();
        if (ordinationList.Count == 0)
            throw new ValidationException(nameof(ordinationer), "en recept skal indeholde mindst én ordination");

        foreach (var o in ordinationList)
        {
            if (string.IsNullOrWhiteSpace(o.Laegemiddel))
                throw new ValidationException(nameof(o.Laegemiddel), "må ikke være tom");
            if (string.IsNullOrWhiteSpace(o.Dosis))
                throw new ValidationException(nameof(o.Dosis), "må ikke være tom");
            if (o.AntalUdleveringer <= 0)
                throw new ValidationException(nameof(o.AntalUdleveringer), "skal være > 0");
        }

        var laegehusExists = await _db.Laegehuse.AnyAsync(l => l.Ydernummer == ydernummer);
        if (!laegehusExists)
            throw new NotFoundException(nameof(Laegehus), ydernummer);

        if (tilknyttetApotekId.HasValue)
        {
            var apotekExists = await _db.Apoteker.AnyAsync(a => a.Id == tilknyttetApotekId.Value);
            if (!apotekExists)
                throw new NotFoundException(nameof(Apotek), tilknyttetApotekId.Value.ToString());
        }

        var recept = new Recept
        {
            Ydernummer = ydernummer,
            CprNummer = normalizedCpr,
            Oprettelsesdato = DateTime.UtcNow,
            Lukket = false,
            TilknyttetApotekId = tilknyttetApotekId,
            Ordinationer = ordinationList.Select(o => new Ordination
            {
                Laegemiddel = o.Laegemiddel,
                Dosis = o.Dosis,
                AntalUdleveringer = o.AntalUdleveringer,
                AntalForetagneUdleveringer = 0
            }).ToList()
        };

        _db.Recepter.Add(recept);
        await _db.SaveChangesAsync();
        return recept;
    }

    public async Task<IReadOnlyList<Recept>> FindAktiveRecepterForCprAsync(string cprNummer)
    {
        var normalizedCpr = NormalizeCpr(cprNummer);

        return await _db.Recepter
            .Include(r => r.Ordinationer)
            .Include(r => r.TilknyttetApotek)
            .Where(r => r.CprNummer == normalizedCpr && !r.Lukket)
            .OrderByDescending(r => r.Oprettelsesdato)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Recept>> FindAktiveRecepterForApotekAsync(int apotekId)
    {
        return await _db.Recepter
            .Include(r => r.Ordinationer)
            .Include(r => r.TilknyttetApotek)
            .Where(r => r.TilknyttetApotekId == apotekId && !r.Lukket)
            .OrderByDescending(r => r.Oprettelsesdato)
            .ToListAsync();
    }

    public async Task<Ordination> UdleverAsync(int ordinationId)
    {
        var ordination = await _db.Ordinationer
            .Include(o => o.Recept!)
                .ThenInclude(r => r.Ordinationer)
            .FirstOrDefaultAsync(o => o.Id == ordinationId);

        if (ordination is null)
            throw new NotFoundException(nameof(Ordination), ordinationId.ToString());

        var recept = ordination.Recept!;

        if (recept.Lukket)
            throw new UdleveringNotAllowedException(ordinationId, "recepten er lukket");

        if (ordination.AntalForetagneUdleveringer >= ordination.AntalUdleveringer)
            throw new UdleveringNotAllowedException(
                ordinationId,
                $"alle {ordination.AntalUdleveringer} udleveringer er allerede foretaget");

        ordination.AntalForetagneUdleveringer++;

        var alleFuldtUdleveret = recept.Ordinationer
            .All(o => o.AntalForetagneUdleveringer >= o.AntalUdleveringer);
        if (alleFuldtUdleveret)
        {
            recept.Lukket = true;
        }

        await _db.SaveChangesAsync();
        return ordination;
    }

    private static string NormalizeCpr(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ValidationException("cprNummer", "må ikke være tom");

        var normalized = input.Trim().Replace("-", "");
        if (normalized.Length != 10 || !normalized.All(char.IsDigit))
            throw new ValidationException("cprNummer", "skal være 10 cifre (med eller uden bindestreg)");

        return normalized;
    }
}