using Broadcaster.Business;
using Microsoft.AspNetCore.Mvc;

namespace Broadcaster.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatusController : ControllerBase
{
    private readonly ILogger<StatusController> _logger;
    private readonly UnitService _unitService;
    private readonly PlaceholderService _placeholderService;

    public StatusController(
        ILogger<StatusController> logger,
        UnitService unitService,
        PlaceholderService placeholderService)
    {
        _logger = logger;
        _unitService = unitService;
        _placeholderService = placeholderService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(string), 200)]
    public async Task<ActionResult> Get()
    {
        List<string> units = await _unitService.ListAsync();
        if (units.Count == 1 && units.First() == "Stake")
        {
            await _unitService.DeleteAsync("Stake");
            await _unitService.InsertAsync("West Stake");
            await _unitService.InsertAsync("Brentwood");
            await _unitService.InsertAsync("Taylor Crossing");
            await _unitService.InsertAsync("Westhill");
        }

        return Ok("Initalized");
    }
}

