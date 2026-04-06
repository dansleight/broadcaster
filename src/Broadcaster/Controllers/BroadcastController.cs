using Broadcaster.SpaModels;
using Broadcaster.Stream;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Contacts.Item.Manager;

namespace Broadcaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BroadcastController : ControllerBase
    {
        private readonly ILogger<BroadcastController> _logger;
        private readonly StreamManager _manager;
        private readonly IAudioLevelNotifier _audioLevelNotifier;

        public BroadcastController(
            ILogger<BroadcastController> logger,
            StreamManager manager,
            IAudioLevelNotifier audioLevelNotifier
        )
        {
            _logger = logger;
            _manager = manager;
            _audioLevelNotifier = audioLevelNotifier;
        }

        [HttpGet("test")]
        [ProducesResponseType(typeof(bool), 200)]
        public ActionResult Test()
        {
            bool res = _manager.TestMatch();
            return Ok(res);
        }

        [HttpGet("audio-test")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<ActionResult> AudioTest()
        {
            await _audioLevelNotifier.NotifyAsync(.5);
            return Ok(true);
        }

        [HttpGet("current-task")]
        [ProducesResponseType(typeof(string), 200)]
        public ActionResult GetCurrentTask()
        {
            var ct = _manager.GetCurrentTask();

            return Ok(ct?.ToString() ?? "none");
        }

        [HttpGet("set-placeholder/{usealt}")]
        [ProducesResponseType(typeof(StreamStatusModel), 200)]
        public async Task<ActionResult> SetPlaceholder(bool usealt = false)
        {
            var image = "/tmp/broadcast-will-begin-shortly.jpg";
            var audio = "/tmp/hymn-100-choir.mp3";

            if (usealt)
            {
                image = "/tmp/please-wait-sacrament.jpg";
                audio = "/tmp/hymn-169.mp3";
            }

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
}
