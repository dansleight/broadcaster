using Broadcaster.Business;
using Broadcaster.Business.Models;
using Broadcaster.Business.Services;
using Broadcaster.Classes;
using Broadcaster.SpaModels;
using Microsoft.AspNetCore.Mvc;

namespace Broadcaster.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatusController : ControllerBase
{
    private readonly ILogger<StatusController> _logger;
    private readonly UnitService _unitService;
    private readonly PlaceholderService _placeholderService;
    private readonly RefreshTokenService _refreshTokenService;

    public StatusController(
        ILogger<StatusController> logger,
        UnitService unitService,
        PlaceholderService placeholderService,
        RefreshTokenService refreshTokenService)
    {
        _logger = logger;
        _unitService = unitService;
        _placeholderService = placeholderService;
        _refreshTokenService = refreshTokenService;
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

    [HttpGet("refresh-token")]
    [ProducesResponseType(typeof(string), 200)]
    public async Task<ActionResult> GetRefreshToken()
    {
        string token = TokenHelper.GenerateRefreshToken();
        string hash = TokenHelper.HashToken(token);

        RefreshTokenObject refreshToken = new RefreshTokenObject(
            "dansleight@gmail.com",
            hash
        );

        await _refreshTokenService.InsertAsync(refreshToken);

        return Ok(token);
    }

    [HttpPost("validate-token")]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<ActionResult> ValidateRefreshToken(TokenModel model)
    {
        string hash = TokenHelper.HashToken(model.Token);
        RefreshTokenObject? refreshToken = await _refreshTokenService.GetAsync(hash);
        if (refreshToken == null) return NotFound();
        else if (refreshToken.TokenExpires < DateTime.Now) return Ok(false);
        else return Ok(true);
    }

    [HttpGet("db-path")]
    [ProducesResponseType(typeof(string), 200)]
    public async Task<ActionResult> DbPath()
    {
        return Ok(Path.GetFullPath("~/db/broadcaster.db"));
    }
}

