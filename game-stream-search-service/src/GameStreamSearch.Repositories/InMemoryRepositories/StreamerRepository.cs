using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Repositories.InMemoryRepositories
{
    public class StreamerRepository : IStreamerRepository
    {
        private Dictionary<string, StreamerDto> streamerStore;

        public StreamerRepository()
        {
            streamerStore = new Dictionary<string, StreamerDto>();
        }

        public Task<StreamerDto> GetStreamerById(string streamerId)
        {
            if (!streamerStore.ContainsKey(streamerId))
            {
                return Task.FromResult<StreamerDto>(null);
            }

            return Task.FromResult(streamerStore[streamerId]);
        }

        public Task SaveStreamer(StreamerDto streamer)
        {
            streamerStore.Add(streamer.Id, streamer);

            return Task.FromResult<object>(null);
        }

        public Task<IEnumerable<StreamerDto>> GetStreamers()
        {
            return Task.FromResult<IEnumerable<StreamerDto>>(streamerStore.Values.ToList());
        }

        public Task<StreamerDto> GetStreamerByNameAndPlatform(string streamerName, StreamingPlatform streamingPlatform)
        {
            var streamer = streamerStore.Values.FirstOrDefault(s => s.Name.CompareTo(streamerName) == 0 && s.Platform == streamingPlatform);

            return Task.FromResult(streamer);
        }
    }
}
