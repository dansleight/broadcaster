using Broadcaster.Business;
using Broadcaster.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broadcaster.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class InfoController : ControllerBase
{
    private readonly ILogger<InfoController> _logger;
    private readonly UserService _userService;
    private readonly UnitService _unitService;
    private readonly PlaceholderService _placeholderService;

    public InfoController(
        ILogger<InfoController> logger,
        UserService userService,
        UnitService unitService,
        PlaceholderService placeholderService
    )
    {
        _logger = logger;
        _userService = userService;
        _unitService = unitService;
        _placeholderService = placeholderService;
    }

    [HttpGet("User")]
    [ProducesResponseType(typeof(UserObject), 200)]
    public async Task<ActionResult> GetUser()
    {
        try
        {
            bool isAdmin = User.IsInRole(UserRole.Admin.ToString());
            bool isTech = User.IsInRole(UserRole.Tech.ToString());
            bool isStake = User.IsInRole("West Stake");
            bool isBrentwood = User.IsInRole("Brentwood");
            string email = User.GetEmail();
            UserObject? res = await _userService.GetAsync(email);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("Units")]
    [ProducesResponseType(typeof(List<string>), 200)]
    public async Task<ActionResult> GetUnits()
    {
        try
        {
            return Ok(await GetValidUnitsAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("Placeholders")]
    [ProducesResponseType(typeof(List<PlaceholderObject>), 200)]
    public async Task<ActionResult> GetPlaceholders()
    {
        try
        {
            return Ok(await _placeholderService.ListAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("Placeholders/{unit}")]
    [ProducesResponseType(typeof(List<PlaceholderObject>), 200)]
    public async Task<ActionResult> GetUnitPlaceholders(string unit)
    {
        try
        {
            if (!User.IsInRole(unit) && !User.IsInRole(UserRole.Admin.ToString()))
                return Unauthorized();
            return Ok(await _placeholderService.ListForUnitAsync(unit));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            return StatusCode(500, ex.Message);
        }
    }

    #region Helpers

    private async Task<List<string>> GetValidUnitsAsync()
    {
        List<string> allUnits = await _unitService.ListAsync();
        if (User.IsInRole(UserRole.Admin.ToString()))
            return allUnits;
        return allUnits.Where(x => User.IsInRole(x)).ToList();
    }

    #endregion
}

