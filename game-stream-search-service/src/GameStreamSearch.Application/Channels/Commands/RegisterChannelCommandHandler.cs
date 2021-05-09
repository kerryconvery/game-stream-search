using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Services.StreamProvider;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.Common;
using GameStreamSearch.Domain.Channel;

namespace GameStreamSearch.Application.RegisterOrUpdateChannel
{
    public class RegisterChannelCommand
    {
        public string ChannelName { get; init; }
        public string StreamPlatformName { get; init; }
    }

    public class RegisterChannelCommandHandler : ICommandHandler<RegisterChannelCommand>
    {
        private readonly ChannelRepository channelRepository;
        private readonly StreamPlatformService streamPlatformService;

        public RegisterChannelCommandHandler(ChannelRepository channelRepository, StreamPlatformService streamPlatformService)
        {
            this.channelRepository = channelRepository;
            this.streamPlatformService = streamPlatformService;
        }

        public async Task Handle(RegisterChannelCommand request)
        {
            var maybeChannel = await streamPlatformService.GetPlatformChannel(request.StreamPlatformName, request.ChannelName);

            if (maybeChannel.IsNothing)
            {
                throw new ChannelNotFoundException(request.ChannelName, request.StreamPlatformName);
            };

            maybeChannel.Select(SaveChannel);
        }

        private Task SaveChannel(PlatformChannelDto platformChannel)
        {
            var channel = platformChannel.ToChannel(DateTime.UtcNow);

            return channelRepository.Add(channel);
        }
    }
}
