using System;
using System.Threading.Tasks;
using GameStreamSearch.Api.Contracts;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Commands;
using GameStreamSearch.Application.Models;
using GameStreamSearch.Application.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameStreamSearch.Api.Controllers
{
    public class GetChannelParams
    {
        public string ChannelName { get; init; }
        public string PlatformName { get; init; }
    }

    [ApiController]
    [Route("api")]
    public class ChannelsController : ControllerBase
    {
        private readonly ICommandHandler<RegisterOrUpdateChannelCommand, RegisterOrUpdateChannelCommandResult> upsertChannelCommand;
        private readonly IChannelRepository channelRepository;
        private readonly ITimeProvider timeProvider;

        public ChannelsController(
            ICommandHandler<RegisterOrUpdateChannelCommand, RegisterOrUpdateChannelCommandResult> upsertChannelCommand,
            IChannelRepository channelRepository,
            ITimeProvider timeProvider)
        {
            this.upsertChannelCommand = upsertChannelCommand;
            this.channelRepository = channelRepository;
            this.timeProvider = timeProvider;
        }

        [HttpPut]
        [Route("channels/{platformName}/{channelName}")]
        public async Task<IActionResult> RegisterOrUpdateChannel([FromRoute] string platformName, string channelName)
        {
            var command = new RegisterOrUpdateChannelCommand
            {
                ChannelName = channelName,
                StreamPlatformName = platformName,
                RegistrationDate = timeProvider.GetNow(),
            };

            var commandResult = await upsertChannelCommand.Handle(command);

            switch (commandResult)
            {
                case RegisterOrUpdateChannelCommandResult.ChannelNotFoundOnPlatform:
                    return PresentChannelNotFoundOnPlatform(platformName, channelName);
                case RegisterOrUpdateChannelCommandResult.ChannelAdded:
                    return PresentChannelAdded(platformName, channelName);
                case RegisterOrUpdateChannelCommandResult.ChannelUpdated:
                    return new NoContentResult();
                case RegisterOrUpdateChannelCommandResult.PlatformServiceIsNotAvailable:
                    return PresentPlatformServiceIsUnavilable(platformName);
                default:
                    throw new ArgumentException($"Unsupported channel upsert result {commandResult.ToString()}");
            }
        }

        private IActionResult PresentChannelAdded(string platformName, string channelName)
        {
            var urlParams = new GetChannelParams
            {
                ChannelName = channelName,
                PlatformName = platformName,
            };

            return new CreatedResult(Url.Link(nameof(GetChannel), urlParams), null);
        }

        private IActionResult PresentChannelNotFoundOnPlatform(string platformId, string channelName)
        {
            var errorResponse = new ErrorResponseContract()
                .AddError(new ErrorContract
                {
                    ErrorCode = ErrorCodeType.ChannelNotFoundOnPlatform,
                    ErrorMessage = $"Channel {channelName} not found on {platformId}"
                });

            return new BadRequestObjectResult(errorResponse);
        }

        private IActionResult PresentPlatformServiceIsUnavilable(string platformId)
        {
            var errorResponse = new ErrorResponseContract()
                .AddError(new ErrorContract
                {
                    ErrorCode = ErrorCodeType.PlatformServiceIsNotAvailable,
                    ErrorMessage = $"The platform {platformId} is not available at this time"
                });

            return StatusCode(StatusCodes.Status424FailedDependency, errorResponse);
        }

        [HttpGet]
        [Route("channels")]
        public async Task<IActionResult> GetChannels()
        {
            var channels = await channelRepository.SelectAllChannels();

            return Ok(channels);
        }

        [HttpGet]
        [Route("channels/{platformName}/{channelName}", Name = "GetChannel")]
        public async Task<IActionResult> GetChannel([FromRoute] string platformName, string channelName)
        {
            var getChannelResult = await channelRepository.Get(platformName, channelName);

            return getChannelResult
                .Select<IActionResult>(v => new OkObjectResult(ChannelDto.FromEntity(v)))
                .GetOrElse(new NotFoundResult());
        }
    }
}
