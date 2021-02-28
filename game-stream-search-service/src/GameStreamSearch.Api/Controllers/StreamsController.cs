using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.GetStreams;
using GameStreamSearch.Application.GetStreams.Dto;
using GameStreamSearch.Application.Services.StreamProvider;
using GameStreamSearch.Application.StreamProvider;
using Microsoft.AspNetCore.Mvc;

namespace GameStreamSearch.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class StreamsController : ControllerBase
    {
        private readonly StreamPlatformService streamService;
        private readonly IQueryHandler<GetStreamsQuery, AggregatedStreamsDto> streamsQueryHandler;

        public StreamsController(StreamPlatformService streamService, IQueryHandler<GetStreamsQuery, AggregatedStreamsDto> streamsQueryHandler)
        {
            this.streamService = streamService;
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
                StreamPlatformNames = streamService.GetSupportingPlatforms(filterOptions),
                PageSize = pageSize,
                PageToken = pageToken,
                Filters = new StreamFilters { GameName = filterOptions.GameName },
            };

            var streams = await streamsQueryHandler.Execute(streamsQuery);

            return Ok(streams);
        }
    }
}
