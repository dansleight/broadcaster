using Microsoft.AspNetCore.Mvc;
using Broadcaster.SpaModels;
using Broadcaster.Business;
using Broadcaster.Stream;

namespace Broadcaster.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SettingsController : ControllerBase
{
    private readonly ILogger<SettingsController> _logger;
    private readonly IConfiguration _configuration;
    private readonly StreamManager _manager;

    public SettingsController(
        ILogger<SettingsController> logger,
        IConfiguration configuration,
        StreamManager manager)
    {
        _logger = logger;
        _configuration = configuration;
        _manager = manager;
    }

    /// <summary>
    /// Gets the global settings necessary for the SPA to start, including MSAL settings for Authentication
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(GlobalSettingsModel), 200)]
    public ActionResult Get()
    {
        try
        {
            string applicationMode = "Development";
            var model = new GlobalSettingsModel(applicationMode, _configuration);
            return Ok(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            return StatusCode(500, ex);
        }
    }
}

