
using GameStreamSearch.Domain;
using Microsoft.AspNetCore.Mvc;

namespace GameStreamSearch.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class StreamPlatformController : ControllerBase
    {
        private readonly IStreamService streamService;

        public StreamPlatformController(IStreamService streamService)
        {
            this.streamService = streamService;
        }

        [HttpGet]
        [Route("streamplatforms")]
        public IActionResult GetAllStreamPlatforms()
        {
            return Ok(streamService.GetStreamAllPlatforms());
        }
    }
}
