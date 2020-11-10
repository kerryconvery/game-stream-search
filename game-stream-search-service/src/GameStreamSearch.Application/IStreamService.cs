using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;

namespace GameStreamSearch.Application
{
    public interface IStreamService
    {
        Task<GameStreamsDto> GetStreams(StreamFilterOptionsDto filterOptions, int pageSize, string pagination);
    }
}
