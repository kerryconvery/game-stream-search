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

            if (getStreamChannelResult.Outcome == GetStreamerChannelOutcomeType.ChannelNotFound)
            {
                return UpsertChannelResult.ChannelNotFoundOnPlatform;
            }

            var channel = new Channel
            {
                ChannelName = request.ChannelName,
                StreamPlatform = request.StreamPlatform,
                DateRegistered = request.RegistrationDate,
                AvatarUrl = getStreamChannelResult.Channel.AvatarUrl,
                ChannelUrl = getStreamChannelResult.Channel.ChannelUrl,
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
