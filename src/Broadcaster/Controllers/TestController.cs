using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Broadcaster.SpaModels;

namespace Broadcaster.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GoodModel), 200)]
    [ProducesResponseType(typeof(BadRequestModel), 400)]
    public ActionResult Get(int id)
    {
        try
        {
            if (id == -1)
                throw new ArgumentOutOfRangeException("id", "this error was intentionally thrown by specifying '-1' as the id value.");
            if (id < -1)
                return NotFound();
            if (id == 0)
                return BadRequest(new BadRequestModel()
                {
                    Message = "Bad request triggered by id of 0",
                    UserMessage = "The value of '0' that was requested is not available to this user."
                });
            return Ok(new GoodModel(id));

        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

}
