using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Commands
{
    public class UpsertChannelCommand: IUpsertChannelCommand
    {
        private readonly IChannelRepository channelRepository;
        private readonly IStreamService streamService;

        public UpsertChannelCommand(IChannelRepository channelRepository, IStreamService streamService)
        {
            this.channelRepository = channelRepository;
            this.streamService = streamService;
        }

        public async Task<UpsertChannelResult> Invoke(UpsertChannelRequest request)
        {
            var streamChannelResult = await streamService.GetStreamerChannel(request.ChannelName, request.StreamPlatform);

            if (streamChannelResult.IsFailure)
            {
                return UpsertChannelResult.PlatformServiceIsNotAvailable;
            }

            if (streamChannelResult.Value.IsNothing)
            {
                return UpsertChannelResult.ChannelNotFoundOnPlatform;
            }

            var streamChannel = streamChannelResult.Value.Unwrap();

            var channel = new Channel
            {
                ChannelName = request.ChannelName,
                StreamPlatform = request.StreamPlatform,
                DateRegistered = request.RegistrationDate,
                AvatarUrl = streamChannel.AvatarUrl,
                ChannelUrl = streamChannel.ChannelUrl,
            };

            var existingChannel = await channelRepository.Get(request.StreamPlatform, request.ChannelName);

            if (existingChannel.IsJust)
            {
                await channelRepository.Update(channel);

                return UpsertChannelResult.ChannelUpdated;
            }

            await channelRepository.Add(channel);

            return UpsertChannelResult.ChannelAdded;
        }
    }
}
