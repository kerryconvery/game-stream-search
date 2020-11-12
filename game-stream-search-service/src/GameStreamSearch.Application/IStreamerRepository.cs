using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application
{
    public interface IStreamerRepository
    {
        Task SaveStreamer(StreamerDto streamer);
        Task<IEnumerable<StreamerDto>> GetStreamers();
        Task<StreamerDto> GetStreamerById(string streamerId);
        Task<StreamerDto> GetStreamerByNameAndPlatform(string streamerName, StreamingPlatform streamingPlatform);
    }
}
