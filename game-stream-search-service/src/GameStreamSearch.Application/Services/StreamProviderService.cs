using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Services
{
    public struct StreamSource
    {
        public StreamPlatform StreamPlatform { get; init; }
        public string PageToken { get; init; }
    }

    public class StreamProviderService
    {
        private Dictionary<StreamPlatform, IStreamProvider> streamProviders;

        public StreamProviderService()
        {
            streamProviders = new Dictionary<StreamPlatform, IStreamProvider>();
        }

        public StreamProviderService RegisterStreamProvider(IStreamProvider streamProvider)
        {
            streamProviders.Add(streamProvider.StreamPlatform, streamProvider);

            return this;
        }

        public IEnumerable<StreamSource> CreateStreamSources(
            Dictionary<string, string> pageTokens, StreamFilterOptions filterOptions
         )
        {
            return streamProviders
                .Values
                .Where(p => p.AreFilterOptionsSupported(filterOptions))
                .Select(p => new StreamSource {
                    StreamPlatform = p.StreamPlatform,
                    PageToken = pageTokens.GetValueOrDefault(p.StreamPlatform.PlatformId, string.Empty)
                });
        }

        public async Task<IEnumerable<PlatformStreams>> GetStreams(
            IEnumerable<StreamSource> sourcePlatforms, StreamFilterOptions filterOptions, int pageSize
        )
        {
            var tasks = sourcePlatforms.Select(s =>
            {
                return streamProviders[s.StreamPlatform].GetLiveStreams(filterOptions, pageSize, s.PageToken);
            });

            return await Task.WhenAll(tasks);
        }

        public Task<MaybeResult<PlatformChannel, StreamProviderError>> GetStreamerChannel(StreamPlatform streamingPlatform, string streamerName)
        {
            var streamProvider = streamProviders[streamingPlatform];

            return streamProvider.GetStreamerChannel(streamerName);
        }
    }
}
