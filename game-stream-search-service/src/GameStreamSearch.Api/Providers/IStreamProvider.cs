using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Domain.Enums;

namespace GameStreamSearch.Api.Providers
{
    public class StreamFilterOptions
    {
        public string GameName { get; init; }
    }

    public interface IStreamProvider
    {
        Task<GameStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken = null);
        Task<ChannelDto> GetStreamerChannel(string channelName);

        StreamPlatformType Platform { get; }
    }
}
