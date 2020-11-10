using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;

namespace GameStreamSearch.Application.Repositories
{
    public interface IStreamerRepository
    {
        Task SaveStreamer(StreamerDto streamer);
    }
}
