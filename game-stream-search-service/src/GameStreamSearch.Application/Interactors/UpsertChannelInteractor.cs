using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Entities;

namespace GameStreamSearch.Application.Interactors
{
    public class UpsertChannelInteractor : IUpsertChannel
    {
        private readonly IChannelRepository channelRepository;
        private readonly IStreamService streamService;

        public UpsertChannelInteractor(
            IChannelRepository channelRepository,
            IStreamService streamService
        )
        {
            this.channelRepository = channelRepository;
            this.streamService = streamService;
        }

        public async Task<Result> Invoke<Result>(UpsertChannelRequest request, IUpsertChannelPresenter<Result> presenter)
        {
            var streamerChannel = await streamService.GetStreamerChannel(request.ChannelName, request.StreamPlatform);

            if (streamerChannel == null)
            {
                return presenter.PresentChannelNotFoundOnPlatform(request.ChannelName, request.StreamPlatform);
            }

            var channel = new Channel
            {
                ChannelName = request.ChannelName,
                StreamPlatform = request.StreamPlatform,
                DateRegistered = request.RegistrationDate,
                AvatarUrl = streamerChannel.AvatarUrl,
                ChannelUrl = streamerChannel.ChannelUrl,
            };

            var existingChannel = await channelRepository.Get(request.StreamPlatform, request.ChannelName);

            if (existingChannel != null)
            {
                await channelRepository.Update(channel);

                return presenter.PresentChannelUpdated(channel);
            }

            await channelRepository.Add(channel);

            return presenter.PresentChannelAdded(channel);
        }
    }
}
