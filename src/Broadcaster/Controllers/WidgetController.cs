using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Broadcaster.Business;
using Broadcaster.Business.Services;
using Broadcaster.SpaModels;

namespace Broadcaster.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class WidgetController : ControllerBase
{
    private readonly ILogger<WidgetController> _logger;
    private readonly WidgetService _widgetService;

    public WidgetController(
        ILogger<WidgetController> logger,
        WidgetService widgetService
    )
    {
        _logger = logger;
        _widgetService = widgetService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WidgetObject>), 200)]
    public async Task<ActionResult> Get()
    {
        try
        {
            IEnumerable<WidgetObject> widgets = await _widgetService.GetAsync();
            return Ok(widgets);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
            return StatusCode(500, e);
        }
    }

    [HttpGet("{widgetId}")]
    [ProducesResponseType(typeof(WidgetObject), 200)]
    public async Task<ActionResult> Get(int widgetId)
    {
        try
        {
            WidgetObject? widget = await _widgetService.GetByIdAsync(widgetId);
            if (widget == null) return NotFound();
            return Ok(widget);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
            return StatusCode(500, e);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(WidgetObject), 200)]
    [ProducesResponseType(typeof(HttpValidationError), 400)]
    public async Task<ActionResult> Add(AddWidgetModel model)
    {
        try
        {
            if (model.Name.Contains("Dan"))
            {
                return BadRequest(new HttpValidationError("name", "No Dan's are allowed here!"));
            }
            var res = await _widgetService.AddAsync(model.ToWidgetObject());
            return Ok(res);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
            return StatusCode(500, e);
        }
    }
}