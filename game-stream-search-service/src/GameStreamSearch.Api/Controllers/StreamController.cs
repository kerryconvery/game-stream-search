using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GameStreamsDto>> GetStreams(
            [FromQuery(Name = "game")] string gameName = null,
            [FromQuery(Name = "pageSize")] int pageSize = 8,
            [FromQuery(Name = "pageToken")] string pageToken = null)
        {
            var filterOptions = new StreamFilterOptionsDto
            {
                GameName = gameName
            };

            var streams = await streamService.GetStreams(filterOptions, pageSize, pageToken);

            return streams;
        }
    }
}
