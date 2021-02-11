using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.Application.Enums;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Base64Url;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Services
{
    public class ProviderAggregationService: IStreamService, IChannelService
    {
        private Dictionary<StreamPlatformType, IStreamProvider> streamProviders;

        public ProviderAggregationService()
        {
            streamProviders = new Dictionary<StreamPlatformType, IStreamProvider>();
        }

        private Dictionary<StreamPlatformType, string> UnpackPageTokens(string encodedPaginations)
        {
            if (string.IsNullOrEmpty(encodedPaginations))
            {
                return new Dictionary<StreamPlatformType, string>();
            }

            var base64Decrypter = new Base64Decryptor(encodedPaginations, new FromBase64Transform());

            var jsonTokens = base64Decrypter.ReadVarString();

            return JsonConvert.DeserializeObject<Dictionary<StreamPlatformType, string>>(jsonTokens);
        }

        private string PackPageTokens(Dictionary<StreamPlatformType, string> paginations)
        {
            if (!paginations.Any())
            {
                return null;
            }

            var jsonTokens = JsonConvert.SerializeObject(paginations);

            var base64Encryptor = new Base64Encryptor(new ToBase64Transform());

            base64Encryptor.WriteVar(jsonTokens);

            return base64Encryptor.ToString();
        }

        private string AggregateNextPageTokens(Streams[] gameStreams)
        {
            var pageTokens = new Dictionary<StreamPlatformType, string>();

            for (int index = 0; index < gameStreams.Length; index++)
            {
                if (!string.IsNullOrEmpty(gameStreams[index].NextPageToken))
                {
                    pageTokens.Add(streamProviders.Values.ElementAt(index).Platform, gameStreams[index].NextPageToken);
                }
            }

            return PackPageTokens(pageTokens);
        }

        public async Task<Streams> GetStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken)
        {
            var paginationTokens = UnpackPageTokens(pageToken);

            var tasks = streamProviders.Values.Select(p => {
                if (!p.AreFilterOptionsSupports(filterOptions))
                {
                    return Task.FromResult(Streams.Empty);
                }

                var pageToken = paginationTokens.ContainsKey(p.Platform) ? paginationTokens[p.Platform] : string.Empty;

                return p.GetLiveStreams(filterOptions, pageSize, pageToken);
            });

            var results = await Task.WhenAll(tasks);

            var nextPageToken = AggregateNextPageTokens(results);

            var sortedItems = results
                .SelectMany(s => s.Items)
                .OrderByDescending(s => s.Views);

            return new Streams(sortedItems, nextPageToken);
        }

        public ProviderAggregationService RegisterStreamProvider(IStreamProvider streamProvider)
        {
            streamProviders.Add(streamProvider.Platform, streamProvider);

            return this;
        }

        public Task<MaybeResult<PlatformChannel, StreamProviderError>> GetStreamerChannel(string streamerName, StreamPlatformType streamingPlatform)
        {
            var streamProvider = streamProviders[streamingPlatform];

            return streamProvider.GetStreamerChannel(streamerName);
        }
    }
}
