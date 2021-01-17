using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application
{
    public interface IStreamService
    {
        Task<GameStreamsDto> GetStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken);
        Task<GetStreamerChannelResult> GetStreamerChannel(string streamerName, StreamPlatformType streamingPlatform);
        IEnumerable<StreamPlatformDto> GetStreamAllPlatforms();
    }
}
