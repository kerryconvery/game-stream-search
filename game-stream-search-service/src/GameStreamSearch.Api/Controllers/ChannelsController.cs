using System;
using System.Threading.Tasks;
using GameStreamSearch.Api.Contracts;
using GameStreamSearch.Application;
using GameStreamSearch.Application.GetAllChannels;
using GameStreamSearch.Application.GetASingleChannel;
using GameStreamSearch.Application.RegisterOrUpdateChannel;
using GameStreamSearch.Common;
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
        private readonly ICommandHandler<RegisterChannelCommand> registerChannelCommand;
        private readonly IQueryHandler<GetAllChannelsQuery, GetAllChannelsResponse> getAllChannelsQueryHandler;
        private readonly IQueryHandler<GetASingleChannelQuery, GetASingleChannelResponse> getChannelQueryHandler;

        public ChannelsController(
            ICommandHandler<RegisterChannelCommand> registerChannelCommand,
            IQueryHandler<GetAllChannelsQuery, GetAllChannelsResponse> getAllChannelsQueryHandler,
            IQueryHandler<GetASingleChannelQuery, GetASingleChannelResponse> getChannelQueryHandler)
        {
            this.registerChannelCommand = registerChannelCommand;
            this.getAllChannelsQueryHandler = getAllChannelsQueryHandler;
            this.getChannelQueryHandler = getChannelQueryHandler;
        }

        [HttpPut]
        [Route("channels/{platformName}/{channelName}")]
        public async Task<IActionResult> RegisterOrUpdateChannel([FromRoute] string platformName, string channelName)
        {
            try
            {
                return await TryRegisterChannel(platformName, channelName);
            } catch(ChannelNotFoundException exception)
            {
                return PresentChannelNotFoundOnPlatform(exception.StreamPlatformName, exception.ChannelName);
            }
        }

        private async Task<IActionResult> TryRegisterChannel(string platformName, string channelName)
        {
            var command = new RegisterChannelCommand
            {
                ChannelName = channelName,
                StreamPlatformName = platformName,
            };

            await registerChannelCommand.Handle(command);

            return PresentChannelAdded(platformName, channelName);
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

        [HttpGet]
        [Route("channels")]
        public async Task<IActionResult> GetChannels()
        {
            var response = await getAllChannelsQueryHandler.Execute(new GetAllChannelsQuery());

            return Ok(response);
        }

        [HttpGet]
        [Route("channels/{platformName}/{channelName}", Name = "GetChannel")]
        public async Task<IActionResult> GetChannel([FromRoute] string platformName, string channelName)
        {
            var getChannelQuery = new GetASingleChannelQuery { platformName = platformName, channelName = channelName };

            var getChannelResponse = await getChannelQueryHandler.Execute(getChannelQuery);

            return getChannelResponse.channel
                .Select<IActionResult>(v => new OkObjectResult(v))
                .GetOrElse(new NotFoundResult());
        }
    }
}
