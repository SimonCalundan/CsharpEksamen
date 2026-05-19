using Microsoft.AspNetCore.Mvc;
using Receptserver.Core.Dtos;
using Receptserver.Core.Services;

namespace Receptserver.Api.Controllers;

[ApiController]
[Route("api/apotek")]
public class ApotekController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public ApotekController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    // Find aktive (ikke-lukkede) recepter for en patient
    [HttpGet("recepter")]
    public async Task<IReadOnlyList<ReceptDto>> SoegRecepter([FromQuery] string cpr)
    {
        var recepter = await _recipeService.FindAktiveRecepterForCprAsync(cpr);
        return recepter.Select(r => r.ToDto()).ToList();
    }

    // Foretag udlevering på en ordination
    [HttpPost("ordinationer/{id:int}/udlever")]
    public async Task<ActionResult<OrdinationDto>> Udlever(int id)
    {
        var ordination = await _recipeService.UdleverAsync(id);
        return Ok(ordination.ToDto());
    }
}
