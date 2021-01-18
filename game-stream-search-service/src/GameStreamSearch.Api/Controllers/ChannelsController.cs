using System;
using System.Threading.Tasks;
using GameStreamSearch.Api.Contracts;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameStreamSearch.Api.Controllers
{
    public class GetChannelParams
    {
        public string ChannelName { get; init; }
        public StreamPlatformType Platform { get; init; }
    }

    [ApiController]
    [Route("api")]
    public class ChannelsController : ControllerBase
    {
        private readonly IUpsertChannelCommand upsertChannelCommand;
        private readonly IChannelRepository channelRepository;
        private readonly ITimeProvider timeProvider;

        public ChannelsController(
            IUpsertChannelCommand upsertChannelCommand,
            IChannelRepository channelRepository,
            ITimeProvider timeProvider)
        {
            this.upsertChannelCommand = upsertChannelCommand;
            this.channelRepository = channelRepository;
            this.timeProvider = timeProvider;
        }

        private IActionResult PresentChannelAdded(StreamPlatformType platform, string channelName)
        {
            var urlParams = new GetChannelParams
            {
                ChannelName = channelName,
                Platform = platform,
            };

            return new CreatedResult(Url.Link(nameof(GetChannel), urlParams), null);
        }

        private IActionResult PresentChannelNotFoundOnPlatform(StreamPlatformType platform, string channelName)
        {
            var errorResponse = new ErrorResponseContract()
                .AddError(new ErrorContract
                {
                    ErrorCode = ErrorCodeType.ChannelNotFoundOnPlatform,
                    ErrorMessage = $"Channel {channelName} not found on {platform.GetFriendlyName()}"
                });

            return new BadRequestObjectResult(errorResponse);
        }

        private IActionResult PresentPlatformServiceIsUnavilable(StreamPlatformType platform)
        {
            var errorResponse = new ErrorResponseContract()
                .AddError(new ErrorContract
                {
                    ErrorCode = ErrorCodeType.PlatformServiceIsNotAvailable,
                    ErrorMessage = $"The platform {platform.GetFriendlyName()} is not available at this time"
                });

            return StatusCode(StatusCodes.Status424FailedDependency, errorResponse);
        }

        [HttpPut]
        [Route("channels/{platform}/{channelName}")]
        public async Task<IActionResult> AddChannel([FromRoute] StreamPlatformType platform, string channelName)
        {
            var request = new UpsertChannelRequest
            {
                ChannelName = channelName,
                StreamPlatform = platform,
                RegistrationDate = timeProvider.GetNow(),
            };

            var commandResult = await upsertChannelCommand.Invoke(request);

            switch (commandResult)
            {
                case UpsertChannelResult.ChannelNotFoundOnPlatform:
                    return PresentChannelNotFoundOnPlatform(platform, channelName);
                case UpsertChannelResult.ChannelAdded:
                    return PresentChannelAdded(platform, channelName);
                case UpsertChannelResult.ChannelUpdated:
                    return new NoContentResult();
                case UpsertChannelResult.PlatformServiceIsNotAvailable:
                    return PresentPlatformServiceIsUnavilable(platform);
                default:
                    throw new ArgumentException($"Unsupported channel upsert result {commandResult.ToString()}");
            }
        }

        [HttpGet]
        [Route("channels")]
        public async Task<IActionResult> GetChannels()
        {
            var channels = await channelRepository.SelectAllChannels();

            return Ok(channels);
        }

        [HttpGet]
        [Route("channels/{platform}/{channelName}", Name = "GetChannel")]
        public async Task<IActionResult> GetChannel([FromRoute] StreamPlatformType platform, string channelName)
        {
            var result = await channelRepository.Get(platform, channelName);

            return result
                .Map<IActionResult>(v => new OkObjectResult(ChannelDto.FromEntity(v)))
                .GetOrElse(new NotFoundResult());
        }
    }
}
