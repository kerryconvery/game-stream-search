using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application
{
    public class StreamFilterOptions
    {
        public string GameName { get; set; }
    }

    public enum GetStreamerChannelOutcomeType
    {
        ChannelFound,
        ChannelNotFound,
        ProviderNotAvailable,
    }

    public class GetStreamerChannelResult
    {
        public static GetStreamerChannelResult ChannelFound(StreamerChannelDto channel) {
            return new GetStreamerChannelResult {
                Outcome = GetStreamerChannelOutcomeType.ChannelFound,
                Channel = channel
            };
        }

        public static GetStreamerChannelResult ChannelNotFound() {
            return new GetStreamerChannelResult
            {
                Outcome = GetStreamerChannelOutcomeType.ChannelNotFound
            };
        }

        public static GetStreamerChannelResult ProviderNotAvailable()
        {
            return new GetStreamerChannelResult
            {
                Outcome = GetStreamerChannelOutcomeType.ProviderNotAvailable
            };
        }

        public GetStreamerChannelOutcomeType Outcome { get; init; }
        public StreamerChannelDto Channel { get; init; }
    }

    public interface IStreamProvider
    {
        Task<GameStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken = null);
        Task<GetStreamerChannelResult> GetStreamerChannel(string channelName);

        StreamPlatformType Platform { get; }
    }
}
