using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Receptserver.Core.Dtos;
using Receptserver.Core.Persistence;
using Receptserver.Core.Services;

namespace Receptserver.Api.Controllers;

[ApiController]
[Route("api/apotek")]
public class ApotekController : ControllerBase
{
    private readonly IRecipeService _recipeService;
    private readonly IReceptDbContext _db;

    public ApotekController(IRecipeService recipeService, IReceptDbContext db)
    {
        _recipeService = recipeService;
        _db = db;
    }

    // Liste med alle apoteker 
    [HttpGet("apoteker")]
    public async Task<IReadOnlyList<ApotekDto>> GetApoteker()
    {
        var liste = await _db.Apoteker.OrderBy(a => a.Navn).ToListAsync();
        return liste.Select(a => a.ToDto()).ToList();
    }

    // Find aktive (ikke-lukkede) recepter for en patient
    [HttpGet("recepter")]
    public async Task<IReadOnlyList<ReceptDto>> SoegRecepter([FromQuery] string cpr)
    {
        var recepter = await _recipeService.FindAktiveRecepterForCprAsync(cpr);
        return recepter.Select(r => r.ToDto()).ToList();
    }

    // Find aktive recepter tilknyttet et specifikt apotek 
    [HttpGet("apoteker/{apotekId:int}/recepter")]
    public async Task<IReadOnlyList<ReceptDto>> GetRecepterForApotek(int apotekId)
    {
        var recepter = await _recipeService.FindAktiveRecepterForApotekAsync(apotekId);
        return recepter.Select(r => r.ToDto()).ToList();
    }

    // Foretag udlevering på en ordination
    [HttpPost("ordinationer/{ordinationId:int}/udlever")]
    public async Task<ActionResult<OrdinationDto>> Udlever(int ordinationId)
    {
        var ordination = await _recipeService.UdleverAsync(ordinationId);
        return Ok(ordination.ToDto());
    }
}
