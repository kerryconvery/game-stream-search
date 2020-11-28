using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application
{
    public interface IStreamProvider
    {
        Task<GameStreamsDto> GetLiveStreams(StreamFilterOptionsDto filterOptions, int pageSize, string pageToken = null);
        Task<StreamerChannelDto> GetStreamerChannel(string channelName);

        StreamPlatformType Platform { get; }
    }
}
