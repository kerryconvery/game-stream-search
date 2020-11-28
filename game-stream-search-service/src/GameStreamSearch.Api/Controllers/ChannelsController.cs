using System.Threading.Tasks;
using GameStreamSearch.Api.Presenters;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Interactors;
using Microsoft.AspNetCore.Mvc;

namespace GameStreamSearch.Api.Controllers
{
    public class GetChannelParams
    {
        public string ChannelName { get; init; }
        public StreamPlatformType StreamPlatform { get; init; }
    }

    [ApiController]
    [Route("api")]
    public class ChannelsController : ControllerBase
    {
        private readonly UpsertChannelInteractor addChannelInteractor;
        private readonly GetChannelInteractor getChannelInteractor;
        private readonly IChannelRepository platformChannelRepository;
        private readonly ITimeProvider timeProvider;

        public ChannelsController(
            UpsertChannelInteractor addChannelInteractor,
            GetChannelInteractor getChannelInteractor,
            IChannelRepository platformChannelRepository,
            ITimeProvider timeProvider)
        {
            this.addChannelInteractor = addChannelInteractor;
            this.getChannelInteractor = getChannelInteractor;
            this.platformChannelRepository = platformChannelRepository;
            this.timeProvider = timeProvider;
        }

        [HttpPut]
        [Route("channels/{platform}/{channel}")]
        public async Task<IActionResult> AddChannel([FromBody] StreamPlatformType streamPlatform, string channel)
        {
            var request = new UpsertChannelRequest
            {
                ChannelName = channel,
                StreamPlatform = streamPlatform,
                RegistrationDate = timeProvider.GetNow(),
            };

            return await addChannelInteractor.Invoke(request, new UpsertChannelPresenter(this));
        }

        [HttpGet]
        [Route("channels")]
        public async Task<IActionResult> GetChannels()
        {
            var channels = await platformChannelRepository.SelectAllChannels();

            return Ok(channels);
        }

        [HttpGet]
        [Route("channels/{platform}/{channel}", Name = "GetChannel")]
        public Task<IActionResult> GetChannel([FromRoute] StreamPlatformType streamPlatform, string channel)
        {
            return getChannelInteractor.Invoke(streamPlatform, channel, new GetChannelPresenter());
        }
    }
}
