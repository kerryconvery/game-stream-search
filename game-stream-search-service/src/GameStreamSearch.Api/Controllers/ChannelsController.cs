using System;
using System.Threading.Tasks;
using GameStreamSearch.Api.Contracts;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Commands;
using GameStreamSearch.Application.Models;
using GameStreamSearch.Application.Providers;
using GameStreamSearch.Application.Queries;
using GameStreamSearch.Types;
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
        private readonly IQueryHandler<GetAllChannelsQuery, ChannelListDto> getAllChannelsQueryHandler;
        private readonly IQueryHandler<GetChannelQuery, Maybe<ChannelDto>> getChannelQueryHandler;
        private readonly ITimeProvider timeProvider;

        public ChannelsController(
            ICommandHandler<RegisterOrUpdateChannelCommand, RegisterOrUpdateChannelCommandResult> upsertChannelCommand,
            IQueryHandler<GetAllChannelsQuery, ChannelListDto> getAllChannelsQueryHandler,
            IQueryHandler<GetChannelQuery, Maybe<ChannelDto>> getChannelQueryHandler,
            ITimeProvider timeProvider)
        {
            this.upsertChannelCommand = upsertChannelCommand;
            this.getAllChannelsQueryHandler = getAllChannelsQueryHandler;
            this.getChannelQueryHandler = getChannelQueryHandler;
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

        private IActionResult PresentChannelNotFoundOnPlatform(string platformName, string channelName)
        {
            var errorResponse = new ErrorResponseContract()
                .AddError(new ErrorContract
                {
                    ErrorCode = ErrorCodeType.ChannelNotFoundOnPlatform,
                    ErrorMessage = $"Channel {channelName} not found on {platformName}"
                });

            return new BadRequestObjectResult(errorResponse);
        }

        private IActionResult PresentPlatformServiceIsUnavilable(string platformName)
        {
            var errorResponse = new ErrorResponseContract()
                .AddError(new ErrorContract
                {
                    ErrorCode = ErrorCodeType.PlatformServiceIsNotAvailable,
                    ErrorMessage = $"The platform {platformName} is not available at this time"
                });

            return StatusCode(StatusCodes.Status424FailedDependency, errorResponse);
        }

        [HttpGet]
        [Route("channels")]
        public async Task<IActionResult> GetChannels()
        {
            var channels = await getAllChannelsQueryHandler.Execute(new GetAllChannelsQuery());

            return Ok(channels);
        }

        [HttpGet]
        [Route("channels/{platformName}/{channelName}", Name = "GetChannel")]
        public async Task<IActionResult> GetChannel([FromRoute] string platformName, string channelName)
        {
            var getChannelQuery = new GetChannelQuery { platformName = platformName, channelName = channelName };

            var getChannelResult = await getChannelQueryHandler.Execute(getChannelQuery);

            return getChannelResult
                .Select<IActionResult>(v => new OkObjectResult(v))
                .GetOrElse(new NotFoundResult());
        }
    }
}
