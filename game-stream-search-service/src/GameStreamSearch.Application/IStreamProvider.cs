using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;

namespace GameStreamSearch.Application
{
    public interface IStreamProvider
    {
        Task<GameStreamsDto> GetLiveStreams(StreamFilterOptionsDto filterOptions, int pageSize, string pageToken = null);
        Task<StreamerChannelDto> GetStreamerChannel(string channelName);

        string ProviderName { get; }
    }
}
