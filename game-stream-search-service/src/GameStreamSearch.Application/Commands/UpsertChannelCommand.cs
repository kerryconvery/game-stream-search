using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Entities;

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
            var getStreamChannelResult = await streamService.GetStreamerChannel(request.ChannelName, request.StreamPlatform);

            if (getStreamChannelResult.IsFailure && getStreamChannelResult.Error == GetStreamerChannelErrorType.ProviderNotAvailable)
            {
                return UpsertChannelResult.PlatformServiceIsNotAvailable;
            }

            if (getStreamChannelResult.Value == null)
            {
                return UpsertChannelResult.ChannelNotFoundOnPlatform;
            }

            var channel = new Channel
            {
                ChannelName = request.ChannelName,
                StreamPlatform = request.StreamPlatform,
                DateRegistered = request.RegistrationDate,
                AvatarUrl = getStreamChannelResult.Value.AvatarUrl,
                ChannelUrl = getStreamChannelResult.Value.ChannelUrl,
            };

            var existingChannel = await channelRepository.Get(request.StreamPlatform, request.ChannelName);

            if (existingChannel != null)
            {
                await channelRepository.Update(channel);

                return UpsertChannelResult.ChannelUpdated;
            }

            await channelRepository.Add(channel);

            return UpsertChannelResult.ChannelAdded;
        }
    }
}
