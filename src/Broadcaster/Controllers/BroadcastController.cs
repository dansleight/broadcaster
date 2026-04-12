using Broadcaster.Business;
using Broadcaster.SpaModels;
using Broadcaster.Stream;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broadcaster.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BroadcastController : ControllerBase
{
    private readonly ILogger<BroadcastController> _logger;
    private readonly StreamManager _manager;
    private readonly PlaceholderService _placeholderService;

    public BroadcastController(
        ILogger<BroadcastController> logger,
        StreamManager manager,
        PlaceholderService placeholderService
    )
    {
        _logger = logger;
        _manager = manager;
        _placeholderService = placeholderService;
    }

    [HttpGet("current-task")]
    [ProducesResponseType(typeof(string), 200)]
    public ActionResult GetCurrentTask()
    {
        var ct = _manager.GetCurrentTask();

        return Ok(ct?.ToString() ?? "none");
    }

    [HttpPost("set-placeholder")]
    [ProducesResponseType(typeof(StreamStatusModel), 200)]
    public async Task<ActionResult> SetPlaceholder(SetPlaceholderModel model)
    {
        PlaceholderObject? placeholder = await _placeholderService.GetAsync(model.PlaceholderId);
        if (placeholder == null)
            return NotFound();

        if (!(User.IsInRole("Admin") || (!string.IsNullOrEmpty(placeholder.Unit) && User.IsInRole(placeholder.Unit))))
            return Unauthorized();

        var placeholderImage = await _placeholderService.GetImageAsync(model.PlaceholderId);
        var image = Path.Combine("/var/lib/broadcaster", $"{placeholder.PlaceholderId}.jpg");
        await System.IO.File.WriteAllBytesAsync(image, placeholderImage!);

        var audio = "/var/lib/broadcaster/hymn-100-choir.mp3";
        if (model.AudioTrackId % 2 == 1)
            audio = "/var/lib/broadcaster/hymn-169.mp3";

        if (_manager is null)
        {
            _logger.LogInformation("there is no manager");
            return Ok(new { status = "there is no manager" });
        }

        await _manager.RunPlaceholderAsync(image, audio, null);
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
