using System.Threading.Tasks;
using GameStreamSearch.Api.Presenters;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Providers;
using Microsoft.AspNetCore.Mvc;

namespace GameStreamSearch.Api.Controllers
{
    public class GetChannelParams
    {
        public string Channel { get; init; }
        public StreamPlatformType Platform { get; init; }
    }

    [ApiController]
    [Route("api")]
    public class ChannelsController : ControllerBase
    {
        private readonly IUpsertChannel upsertChannel;
        private readonly IGetChannel getChannel;
        private readonly IChannelRepository channelRepository;
        private readonly ITimeProvider timeProvider;

        public ChannelsController(
            IUpsertChannel upsertChannel,
            IGetChannel getChannel,
            IChannelRepository channelRepository,
            ITimeProvider timeProvider)
        {
            this.upsertChannel = upsertChannel;
            this.getChannel = getChannel;
            this.channelRepository = channelRepository;
            this.timeProvider = timeProvider;
        }

        [HttpPut]
        [Route("channels/{platform}/{channel}")]
        public Task<IActionResult> AddChannel([FromRoute] StreamPlatformType platform, string channel)
        {
            var request = new UpsertChannelRequest
            {
                ChannelName = channel,
                StreamPlatform = platform,
                RegistrationDate = timeProvider.GetNow(),
            };

            return upsertChannel.Invoke(request, new UpsertChannelPresenter(this));
        }

        [HttpGet]
        [Route("channels")]
        public async Task<IActionResult> GetChannels()
        {
            var channels = await channelRepository.SelectAllChannels();

            return Ok(channels);
        }

        [HttpGet]
        [Route("channels/{platform}/{channel}", Name = "GetChannel")]
        public Task<IActionResult> GetChannel([FromRoute] StreamPlatformType platform, string channel)
        {
            return getChannel.Invoke(platform, channel, new GetChannelPresenter());
        }
    }
}
