using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Services.StreamProvider;
using GameStreamSearch.Domain.Channel;

namespace GameStreamSearch.Application.RegisterOrUpdateChannel
{
    public class RegisterOrUpdateChannelCommand
    {
        public string ChannelName { get; init; }
        public string StreamPlatformName { get; init; }
    }

    public enum RegisterOrUpdateChannelResult
    {
        ChannelNotFoundOnPlatform,
        ChannelAdded,
        ChannelUpdated,
        PlatformServiceIsNotAvailable,
    }

    public class RegisterOrUpdateChannelResponse
    {
        public RegisterOrUpdateChannelResponse(RegisterOrUpdateChannelResult result)
        {
            Result = result;
        }

        public RegisterOrUpdateChannelResult Result { get; }
    }

    public class RegisterOrUpdateChannelCommandHandler : ICommandHandler<RegisterOrUpdateChannelCommand, RegisterOrUpdateChannelResponse>
    {
        private readonly ChannelRepository channelRepository;
        private readonly StreamPlatformService streamPlatformService;

        public RegisterOrUpdateChannelCommandHandler(ChannelRepository channelRepository, StreamPlatformService streamPlatformService)
        {
            this.channelRepository = channelRepository;
            this.streamPlatformService = streamPlatformService;
        }

        public async Task<RegisterOrUpdateChannelResponse> Handle(RegisterOrUpdateChannelCommand request)
        {
            var streamChannelResult = await streamPlatformService.GetPlatformChannel(request.StreamPlatformName, request.ChannelName);

            if (streamChannelResult.IsFailure)
            {
                return new RegisterOrUpdateChannelResponse(RegisterOrUpdateChannelResult.PlatformServiceIsNotAvailable);
            }

            return await streamChannelResult.Value
                .Select(s => s.ToChannel(DateTime.UtcNow))
                .Select(c => UpsertChannel(c))
                .GetOrElse(Task.FromResult(new RegisterOrUpdateChannelResponse(RegisterOrUpdateChannelResult.ChannelNotFoundOnPlatform)));
        }

        private async Task<RegisterOrUpdateChannelResponse> UpsertChannel(Channel channel)
        {
            var existingChannel = await channelRepository.Get(channel.StreamPlatformName, channel.ChannelName);

            if (existingChannel.IsSome)
            {
                await channelRepository.Update(channel);

                return new RegisterOrUpdateChannelResponse(RegisterOrUpdateChannelResult.ChannelUpdated);
            }

            await channelRepository.Add(channel);

            return new RegisterOrUpdateChannelResponse(RegisterOrUpdateChannelResult.ChannelAdded);
        }
    }
}
