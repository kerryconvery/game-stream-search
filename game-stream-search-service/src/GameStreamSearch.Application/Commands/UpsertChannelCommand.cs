using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Services;

namespace GameStreamSearch.Application.Commands
{
    public enum UpsertChannelResult
    {
        ChannelNotFoundOnPlatform,
        ChannelAdded,
        ChannelUpdated,
        PlatformServiceIsNotAvailable,
    }

    public class UpsertChannelRequest
    {
        public string ChannelName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public StreamPlatformType StreamPlatform { get; set; }
    }

    public class UpsertChannelCommand
    {
        private readonly IChannelRepository channelRepository;
        private readonly IChannelService channelService;

        public UpsertChannelCommand(IChannelRepository channelRepository, IChannelService channelService)
        {
            this.channelRepository = channelRepository;
            this.channelService = channelService;
        }

        public async Task<UpsertChannelResult> Invoke(UpsertChannelRequest request)
        {
            var streamChannelResult = await channelService.GetStreamerChannel(request.ChannelName, request.StreamPlatform);

            if (streamChannelResult.IsFailure)
            {
                return UpsertChannelResult.PlatformServiceIsNotAvailable;
            }

            if (streamChannelResult.Value.IsNothing)
            {
                return UpsertChannelResult.ChannelNotFoundOnPlatform;
            }

            var channel = streamChannelResult.Value.Map(streamChannel =>
                new Channel
                {
                    ChannelName = request.ChannelName,
                    StreamPlatform = request.StreamPlatform,
                    DateRegistered = request.RegistrationDate,
                    AvatarUrl = streamChannel.AvatarUrl,
                    ChannelUrl = streamChannel.ChannelUrl,
                }
            ).Unwrap();

            var existingChannel = await channelRepository.Get(request.StreamPlatform, request.ChannelName);

            if (existingChannel.IsSome)
            {
                await channelRepository.Update(channel);

                return UpsertChannelResult.ChannelUpdated;
            }

            await channelRepository.Add(channel);

            return UpsertChannelResult.ChannelAdded;
        }
    }
}
