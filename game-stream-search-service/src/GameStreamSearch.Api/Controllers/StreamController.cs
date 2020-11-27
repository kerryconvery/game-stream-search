using System.Threading.Tasks;
using GameStreamSearch.Domain.Dto;
using GameStreamSearch.Domain;
using Microsoft.AspNetCore.Mvc;

namespace GameStreamSearch.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class StreamController : ControllerBase
    {
        private readonly IStreamService streamService;

        public StreamController(IStreamService streamService)
        {
            this.streamService = streamService;
        }

        [HttpGet]
        [Route("streams")]
        public async Task<IActionResult> GetStreams(
            [FromQuery(Name = "game")] string gameName = null,
            [FromQuery(Name = "pageSize")] int pageSize = 8,
            [FromQuery(Name = "pageToken")] string pageToken = null)
        {
            var filterOptions = new StreamFilterOptionsDto
            {
                GameName = gameName
            };

            var streams = await streamService.GetStreams(filterOptions, pageSize, pageToken);

            return Ok(streams);
        }
    }
}
