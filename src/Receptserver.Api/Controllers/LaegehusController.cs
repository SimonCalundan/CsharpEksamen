using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Receptserver.Core.Dtos;
using Receptserver.Core.Persistence;
using Receptserver.Core.Services;

namespace Receptserver.Api.Controllers;

[ApiController]
[Route("api/laegehus")]
public class LaegehusController : ControllerBase
{
    private readonly IRecipeService _recipeService;
    private readonly IReceptDbContext _db;

    public LaegehusController(IRecipeService recipeService, IReceptDbContext db)
    {
        _recipeService = recipeService;
        _db = db;
    }

    // Liste med alle laegehuse
    [HttpGet("laegehuse")]
    public async Task<IReadOnlyList<LaegehusDto>> GetLaegehuse()
    {
        var liste = await _db.Laegehuse.OrderBy(l => l.Navn).ToListAsync();
        return liste.Select(l => l.ToDto()).ToList();
    }

    // Liste med alle apoteker
    [HttpGet("apoteker")]
    public async Task<IReadOnlyList<ApotekDto>> GetApoteker()
    {
        var liste = await _db.Apoteker.OrderBy(a => a.Navn).ToListAsync();
        return liste.Select(a => a.ToDto()).ToList();
    }

    // Opret recept 
    [HttpPost("recepter")]
    public async Task<ActionResult<ReceptDto>> OpretRecept([FromBody] OpretReceptRequest request)
    {
        var ordinationer = request.Ordinationer
            .Select(o => new OrdinationInput(o.Laegemiddel, o.Dosis, o.AntalUdleveringer));

        var recept = await _recipeService.OpretReceptAsync(
            request.Ydernummer,
            request.CprNummer,
            ordinationer,
            request.TilknyttetApotekId);

        return CreatedAtAction(nameof(OpretRecept), new { id = recept.Id }, recept.ToDto());
    }
}
