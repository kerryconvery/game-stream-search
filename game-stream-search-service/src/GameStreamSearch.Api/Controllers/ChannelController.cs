using System.Threading.Tasks;
using GameStreamSearch.Application.CommandHandlers;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Repositories;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Domain.Commands;
using GameStreamSearch.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GameStreamSearch.Application.Controllers
{
    [ApiController]
    [Route("api")]
    public class ChannelController : ControllerBase
    {
        private readonly StreamService streamService;
        private readonly ICommandHandler<RegisterChannelCommand> registerChannelCommandHandler;
        private readonly IChannelRepository channelRepository;

        public ChannelController(
            StreamService streamService,
            ICommandHandler<RegisterChannelCommand> registerChannelCommandHandler,
            IChannelRepository channelRepository)
        {
            this.streamService = streamService;
            this.registerChannelCommandHandler = registerChannelCommandHandler;
            this.channelRepository = channelRepository;
        }

        [HttpPost]
        [Route("channels")]
        public async Task<IActionResult> AddChannel([FromBody] RegisterChannelDto channelToRegister)
        {
            var channel = await streamService.GetChannel(channelToRegister.ChannelName, channelToRegister.StreamPlatform);

            if (channel == null)
            {
                return BadRequest($"The channel {channelToRegister.ChannelName} was not found on the stream platform {channelToRegister.StreamPlatform.GetFriendlyName()}");
            }

            var existingChannel = await channelRepository.GetChannelByNameAndPlatform(channelToRegister.ChannelName, channelToRegister.StreamPlatform);

            if (existingChannel != null)
            {
                return Created(Url.Link(nameof(this.GetPlatformChannel), new
                {
                    platform = channelToRegister.StreamPlatform.GetFriendlyName(),
                    name = existingChannel.ChannelName
                }), null);
            }

            var registerChannelCommand = new RegisterChannelCommand
            {
                ChannelName = channelToRegister.ChannelName,
                StreamPlatform = channelToRegister.StreamPlatform,
            };

            await registerChannelCommandHandler.Handle(registerChannelCommand);

            return Created(Url.Link(nameof(this.GetPlatformChannel), new {
                platform = channelToRegister.StreamPlatform.GetFriendlyName(),
                name = channelToRegister.ChannelName
            }), null);
        }

        [HttpGet]
        [Route("channels")]
        public async Task<IActionResult> GetAllChannels()
        {
            var streamers = await channelRepository.GetAllChannels();

            return Ok(streamers);
        }

        [HttpGet]
        [Route("channels/{platform}/{name}", Name = "GetChannel")]
        public IActionResult GetPlatformChannel([FromRoute] string platform, string name)
        {
            return NoContent();
        }
    }
}
