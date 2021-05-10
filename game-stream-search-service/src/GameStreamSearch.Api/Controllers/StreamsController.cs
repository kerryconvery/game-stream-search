using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.GetStreams;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Application.StreamProvider;
using GameStreamSearch.Common;
using Microsoft.AspNetCore.Mvc;

namespace GameStreamSearch.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class StreamsController : ControllerBase
    {
        private readonly IQueryHandler<GetStreamsQuery, GetStreamsQueryResponseDto> streamsQueryHandler;

        public StreamsController(IQueryHandler<GetStreamsQuery, GetStreamsQueryResponseDto> streamsQueryHandler)
        {
            this.streamsQueryHandler = streamsQueryHandler;
        }

        [HttpGet]
        [Route("streams")]
        public async Task<IActionResult> GetStreams(
            [FromQuery(Name = "game")] string gameName = null,
            [FromQuery(Name = "pageSize")] int pageSize = 8,
            [FromQuery(Name = "pageToken")] string pageToken = null)
        {
            var filterOptions = new StreamFilterOptions
            {
                GameName = gameName
            };

            var streamsQuery = new GetStreamsQuery
            {
                PageSize = pageSize,
                PageToken = pageToken,
                Filters = new StreamFilterOptions { GameName = filterOptions.GameName },
            };

            var streams = await streamsQueryHandler.Execute(streamsQuery);

            return Ok(streams);
        }
    }
}
