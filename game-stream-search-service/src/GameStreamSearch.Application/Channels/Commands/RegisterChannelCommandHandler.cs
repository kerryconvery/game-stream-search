using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Services.StreamProvider;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.Domain.Channel;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.RegisterOrUpdateChannel
{
    public class RegisterChannelCommand
    {
        public string ChannelName { get; init; }
        public string StreamPlatformName { get; init; }
    }

    public enum RegisterChannelCommandErrorType
    {
        ChannelNotFound,
    }

    public class RegisterChannelCommandHandler : ICommandHandler<RegisterChannelCommand, RegisterChannelCommandErrorType>
    {
        private readonly ChannelRepository channelRepository;
        private readonly StreamPlatformService streamPlatformService;

        public RegisterChannelCommandHandler(ChannelRepository channelRepository, StreamPlatformService streamPlatformService)
        {
            this.channelRepository = channelRepository;
            this.streamPlatformService = streamPlatformService;
        }

        public async Task<Result<RegisterChannelCommandErrorType>> Handle(RegisterChannelCommand request)
        {
            var maybeChannel = await streamPlatformService.GetPlatformChannel(request.StreamPlatformName, request.ChannelName);

            if (maybeChannel.IsNothing)
            {
                return Result<RegisterChannelCommandErrorType>.Fail(RegisterChannelCommandErrorType.ChannelNotFound);
            };

            maybeChannel.Select(SaveChannel);

            return Result<RegisterChannelCommandErrorType>.Success();
        }

        private Task SaveChannel(PlatformChannelDto platformChannel)
        {
            var channel = platformChannel.ToChannel(DateTime.UtcNow);

            return channelRepository.Add(channel);
        }
    }
}
