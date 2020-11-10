using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;

namespace GameStreamSearch.Application.Services
{
    public class StreamAggregationService : IStreamService
    {
        private readonly IPaginator paginator;
        private List<IStreamProvider> streamProviders;

        public StreamAggregationService(IPaginator paginator)
        {
            streamProviders = new List<IStreamProvider>();
            this.paginator = paginator;
        }

        private string AggregateNextPageTokens(GameStreamsDto[] gameStreams)
        {
            var paginations = new Dictionary<string, string>();

            for (int index = 0; index < gameStreams.Length; index++)
            {
                if (gameStreams[index].NextPageToken != null)
                {
                    paginations.Add(streamProviders[index].ProviderName, gameStreams[index].NextPageToken);
                }
            }

            return paginator.encode(paginations);
        }

        public async Task<GameStreamsDto> GetStreams(StreamFilterOptionsDto filterOptions, int pageSize, string pagination)
        {
            var paginationTokens = paginator.decode(pagination);

            var tasks = streamProviders.Select(p => {
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

        public StreamAggregationService RegisterStreamProvider(IStreamProvider streamProvider)
        {
            streamProviders.Add(streamProvider);

            return this;
        }
    }
}
