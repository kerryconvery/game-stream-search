using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Models;
using GameStreamSearch.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace GameStreamSearch.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class StreamsController : ControllerBase
    {
        private readonly IStreamService streamService;
        private readonly IQueryHandler<StreamsQuery, AggregatedStreamsDto> streamsQueryHandler;

        public StreamsController(IStreamService streamService, IQueryHandler<StreamsQuery, AggregatedStreamsDto> streamsQueryHandler)
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

            var streamsQuery = new StreamsQuery
            {
                StreamPlatformNames = streamService.GetSupportingPlatforms(filterOptions),
                PageSize = pageSize,
                PageToken = pageToken,
                FilterOptions = filterOptions,
            };

            var streams = await streamsQueryHandler.Execute(streamsQuery);

            return Ok(streams);
        }
    }
}
