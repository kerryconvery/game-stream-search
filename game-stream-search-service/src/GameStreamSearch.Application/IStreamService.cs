using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application
{
    public interface IStreamService
    {
        Task<GameStreamsDto> GetStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken);
        Task<Result<StreamerChannelDto?, GetStreamerChannelErrorType>> GetStreamerChannel(string streamerName, StreamPlatformType streamingPlatform);
        IEnumerable<StreamPlatformDto> GetStreamAllPlatforms();
    }
}
