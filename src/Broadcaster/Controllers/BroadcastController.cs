using Broadcaster.Business;
using Broadcaster.Common;
using Broadcaster.SpaModels;
using Broadcaster.Stream;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Broadcaster.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BroadcastController : ControllerBase
{
    private readonly ILogger<BroadcastController> _logger;
    private readonly StreamManager _manager;
    private readonly ArtifactHelper _artifactHelper;

    public BroadcastController(
        ILogger<BroadcastController> logger,
        StreamManager manager,
        PlaceholderService placeholderService,
        ArtifactHelper artifactHelper
    )
    {
        _logger = logger;
        _manager = manager;
        _artifactHelper = artifactHelper;
    }

    [HttpGet("current-task")]
    [ProducesResponseType(typeof(FullStreamState), 200)]
    public ActionResult GetStreamState()
    {
        FullStreamState fss = _manager.FullStreamState;

        return Ok(fss);
    }

    [HttpPost("set-placeholder")]
    [ProducesResponseType(typeof(StreamStatusModel), 200)]
    public async Task<ActionResult> SetPlaceholder(SetPlaceholderModel model)
    {

        if (_manager is null)
        {
            _logger.LogInformation("there is no manager");
            return Ok(new { status = "there is no manager" });
        }

        await _manager.RunPlaceholderAsync(model.PlaceholderType.FilePath(), (model.MusicType ?? MusicType.Default).FileDirectory(), null);
        return Ok(new StreamStatusModel("placeholder started"));
    }

    [HttpGet("set-live")]
    [ProducesResponseType(typeof(StreamStatusModel), 200)]
    public async Task<ActionResult> SetLive()
    {
        if (_manager is null)
        {
            _logger.LogInformation("there is no manager");
            return Ok(new { status = "there is no manager" });
        }

        await _manager.RunLiveVideoAsync();
        return Ok(new StreamStatusModel("live video started"));
    }

    [HttpDelete("stop-all")]
    [ProducesResponseType(typeof(StreamStatusModel), 200)]
    public async Task<ActionResult> StopAll()
    {
        if (_manager is null)
        {
            _logger.LogInformation("there is no manager");
            return Ok(new { status = "there is no manager" });
        }

        await _manager.StopAsync();
        return Ok(new StreamStatusModel("tried to end the broadcast"));
    }

    [HttpGet("schedule-dummy")]
    [ProducesResponseType(typeof(StreamStatusModel), 200)]
    public async Task<ActionResult> ScheduleMeetings()
    {
        return Ok(new StreamStatusModel("nothing to schedule"));
    }



}
