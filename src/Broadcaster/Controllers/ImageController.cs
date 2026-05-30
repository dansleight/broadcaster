using Broadcaster.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Broadcaster.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly ILogger<ImageController> _logger;
    private readonly ArtifactHelper _artifactHelper;

    public ImageController(ILogger<ImageController> logger, ArtifactHelper artifactHelper)
    {
        _logger = logger;
        _artifactHelper = artifactHelper;
    }

    [HttpGet("{imageName}")]
    public ActionResult Image(string imageName)
    {
        string path = Path.Combine(_artifactHelper.ArtifactPath, "slides", imageName);
        byte[] file = System.IO.File.ReadAllBytes(path);
        return File(file, "image/jpeg");
    }
}

