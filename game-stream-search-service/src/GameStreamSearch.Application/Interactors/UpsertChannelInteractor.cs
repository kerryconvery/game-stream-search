using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Services;

namespace GameStreamSearch.Application.Interactors
{
    public class UpsertChannelRequest
    {
        public string ChannelName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public StreamPlatformType StreamPlatform { get; set; }
    }

    public interface IUpsertChannelPresenter<Result>
    {
        Result PresentChannelAdded(string channelName, StreamPlatformType streamPlatform);
        Result PresentChannelUpdated();
        Result PresentChannelNotFoundOnPlatform(string channelName, StreamPlatformType platform);
    }

    public class UpsertChannelInteractor
    {
        private readonly IChannelRepository channelRepository;
        private readonly StreamService streamService;

        public UpsertChannelInteractor(
            IChannelRepository channelRepository,
            StreamService streamService
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

            var platformChannel = new Channel
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
                await channelRepository.Remove(existingChannel.StreamPlatform, existingChannel.ChannelName);

                await channelRepository.Add(platformChannel);

                return presenter.PresentChannelUpdated();
            }

            await channelRepository.Add(platformChannel);

            return presenter.PresentChannelAdded(request.ChannelName, request.StreamPlatform);
        }
    }
}
