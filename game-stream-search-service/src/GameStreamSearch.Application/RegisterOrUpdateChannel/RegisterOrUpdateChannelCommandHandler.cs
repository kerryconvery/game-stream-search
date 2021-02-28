using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Services.StreamProvider;
using GameStreamSearch.Domain.Channel;

namespace GameStreamSearch.Application.RegisterOrUpdateChannel
{
    public enum RegisterOrUpdateChannelCommandResult
    {
        ChannelNotFoundOnPlatform,
        ChannelAdded,
        ChannelUpdated,
        PlatformServiceIsNotAvailable,
    }

    public class RegisterOrUpdateChannelCommandHandler : ICommandHandler<RegisterOrUpdateChannelCommand, RegisterOrUpdateChannelCommandResult>
    {
        private readonly ChannelRepository channelRepository;
        private readonly StreamPlatformService streamPlatformService;

        public RegisterOrUpdateChannelCommandHandler(ChannelRepository channelRepository, StreamPlatformService streamPlatformService)
        {
            this.channelRepository = channelRepository;
            this.streamPlatformService = streamPlatformService;
        }

        private async Task<RegisterOrUpdateChannelCommandResult> UpsertChannel(Channel channel)
        {
            var existingChannel = await channelRepository.Get(channel.StreamPlatformName, channel.ChannelName);

            if (existingChannel.IsSome)
            {
                await channelRepository.Update(channel);

                return RegisterOrUpdateChannelCommandResult.ChannelUpdated;
            }

            await channelRepository.Add(channel);

            return RegisterOrUpdateChannelCommandResult.ChannelAdded;
        }

        public async Task<RegisterOrUpdateChannelCommandResult> Handle(RegisterOrUpdateChannelCommand request)
        {
            var streamChannelResult = await streamPlatformService.GetPlatformChannel(request.StreamPlatformName, request.ChannelName);

            if (streamChannelResult.IsFailure)
            {
                return RegisterOrUpdateChannelCommandResult.PlatformServiceIsNotAvailable;
            }

            return await streamChannelResult.Value
                .Select(s => s.ToChannel(DateTime.UtcNow))
                .Select(c => UpsertChannel(c))
                .GetOrElse(Task.FromResult(RegisterOrUpdateChannelCommandResult.ChannelNotFoundOnPlatform));
        }

    }
}
