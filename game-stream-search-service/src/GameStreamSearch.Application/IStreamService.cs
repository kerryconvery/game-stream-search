using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application
{
    public interface IStreamService
    {
        Task<GameStreamsDto> GetStreams(StreamFilterOptionsDto filterOptions, int pageSize, string pageToken);
        Task<StreamerChannelDto> GetStreamerChannel(string streamerName, StreamingPlatform streamingPlatform);
    }
}
