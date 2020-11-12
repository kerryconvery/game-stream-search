using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.Services
{
    public class StreamService : IStreamService
    {
        private readonly IPaginator paginator;
        private Dictionary<StreamingPlatform, IStreamProvider> streamProviders;

        public StreamService(IPaginator paginator)
        {
            streamProviders = new Dictionary<StreamingPlatform, IStreamProvider>();
            this.paginator = paginator;
        }

        private string AggregateNextPageTokens(GameStreamsDto[] gameStreams)
        {
            var paginations = new Dictionary<string, string>();

            for (int index = 0; index < gameStreams.Length; index++)
            {
                if (gameStreams[index].NextPageToken != null)
                {
                    paginations.Add(streamProviders.Values.ElementAt(index).ProviderName, gameStreams[index].NextPageToken);
                }
            }

            return paginator.encode(paginations);
        }

        public async Task<GameStreamsDto> GetStreams(StreamFilterOptionsDto filterOptions, int pageSize, string pagination)
        {
            var paginationTokens = paginator.decode(pagination);

            var tasks = streamProviders.Values.Select(p => {
                var pageToken = paginationTokens.ContainsKey(p.ProviderName) ? paginationTokens[p.ProviderName] : null;

                return p.GetLiveStreams(filterOptions, pageSize, pageToken);
            });

            var results = await Task.WhenAll(tasks);

            var nextPageToken = AggregateNextPageTokens(results);

            var sortedItems = results
                .SelectMany(s => s.Items)
                .OrderByDescending(s => s.Views);

            return new GameStreamsDto()
            {
                Items = sortedItems,
                NextPageToken = nextPageToken
            };
        }

        public StreamService RegisterStreamProvider(StreamingPlatform streamingPlatform, IStreamProvider streamProvider)
        {
            streamProviders.Add(streamingPlatform, streamProvider);

            return this;
        }

        public Task<StreamerChannelDto> GetStreamerChannel(string streamerName, StreamingPlatform streamingPlatform)
        {
            var streamProvider = streamProviders[streamingPlatform];

            return streamProvider.GetStreamerChannel(streamerName);
        }
    }
}
