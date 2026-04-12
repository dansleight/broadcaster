using Broadcaster.Business;
using Broadcaster.SpaModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
// using Microsoft.Graph.Models;

namespace Broadcaster.Controllers;

[Authorize(Roles = "Admin,Tech")]
[Route("api/[controller]")]
[ApiController]
public class PlaceholderController : ControllerBase
{
    private readonly ILogger<PlaceholderController> _logger;
    private readonly PlaceholderService _placeholderService;

    public PlaceholderController(ILogger<PlaceholderController> logger, PlaceholderService placeholderService)
    {
        _logger = logger;
        _placeholderService = placeholderService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PlaceholderObject>), 200)]
    public async Task<ActionResult> Get()
    {
        try
        {
            if (!User.IsInRole(UserRole.Admin.ToString()))
                return Unauthorized();
            return Ok(await _placeholderService.ListForNoUnitAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("For/{unit}")]
    [ProducesResponseType(typeof(List<PlaceholderObject>), 200)]
    public async Task<ActionResult> GetForUnit(string unit)
    {
        try
        {
            if (!User.IsInRole(unit) && !User.IsInRole(UserRole.Admin.ToString()))
                return Unauthorized();
            List<PlaceholderObject> forUnit = await _placeholderService.ListForUnitAsync(unit);
            return Ok(forUnit.Where(x => x.Unit == unit).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(List<PlaceholderObject>), 200)]
    public async Task<ActionResult> Add(AddPlaceholderModel model)
    {
        try
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest("No file uploaded.");

            if (!model.File.FileName.ToLower().EndsWith(".jpg"))
                return BadRequest("Image File must be a .jpg");

            if (model.File.ContentType != "image/jpeg")
                return BadRequest("Image must have a valid jpeg format");

            if (string.IsNullOrEmpty(model.Unit) && !User.IsInRole(UserRole.Admin.ToString()))
                return Unauthorized();
            else if (!string.IsNullOrEmpty(model.Unit) && !(User.IsInRole(model.Unit) || User.IsInRole(UserRole.Admin.ToString())))
                return Unauthorized();

            byte[] imageBytes = [];
            using (var memoryStream = new MemoryStream())
            {
                await model.File.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset for reading

                using (var image = Image.Load(memoryStream))
                {
                    if (image.Width != 640 || image.Height != 480)
                    {
                        // Resize to exactly 640x480 (stretches if aspect ratio differs)
                        image.Mutate(x => x.Resize(640, 480, KnownResamplers.Lanczos3)); // High quality resampler

                        // Save back to bytes as JPEG (you can adjust quality if desired)
                        using (var outputStream = new MemoryStream())
                        {
                            var encoder = new JpegEncoder { Quality = 90 }; // 90% quality is a good balance
                            image.Save(outputStream, encoder);
                            imageBytes = outputStream.ToArray();
                        }
                    }
                    else
                        imageBytes = memoryStream.ToArray();
                }
            }

            PlaceholderObject placeholder = new PlaceholderObject(model.Unit, model.Name);
            await _placeholderService.InsertAsync(placeholder, imageBytes);
            if (!string.IsNullOrEmpty(model.Unit))
                return Ok(await _placeholderService.ListForUnitAsync(model.Unit));
            return Ok(await _placeholderService.ListForNoUnitAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("/delete/{placeholderId}")]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<ActionResult> Delete(int placeholderId)
    {
        try
        {
            await _placeholderService.DeleteAsync(placeholderId);
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            return StatusCode(500, ex.Message);
        }
    }


}

