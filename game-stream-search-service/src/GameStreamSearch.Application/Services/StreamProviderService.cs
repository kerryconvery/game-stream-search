using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Types;
using GameStreamSearch.Application.Types;

namespace GameStreamSearch.Application.Services
{
    public struct StreamSource
    {
        public string StreamPlatformName { get; init; }
        public string PageToken { get; init; }
    }

    public class StreamProviderService
    {
        private Dictionary<string, IStreamProvider> streamProviders;

        public StreamProviderService()
        {
            streamProviders = new Dictionary<string, IStreamProvider>();
        }

        public StreamProviderService RegisterStreamProvider(IStreamProvider streamProvider)
        {
            streamProviders.Add(streamProvider.StreamPlatform.Name, streamProvider);

            return this;
        }

        public IEnumerable<StreamSource> CreateStreamSources(
            IEnumerable<PageToken> pageTokens, StreamFilterOptions filterOptions
         )
        {
            return streamProviders
                .Values
                .Where(p => p.AreFilterOptionsSupported(filterOptions))
                .Select(p => new StreamSource {
                    StreamPlatformName = p.StreamPlatform,
                    PageToken = pageTokens.SingleOrDefault(t => t.StreamPlatformName == p.StreamPlatform).Token ?? string.Empty,
                });
        }

        public async Task<IEnumerable<PlatformStreamsDto>> GetStreams(
            IEnumerable<StreamSource> sourcePlatforms, StreamFilterOptions filterOptions, int pageSize
        )
        {
            var tasks = sourcePlatforms.Select(s =>
            {
                return streamProviders[s.StreamPlatformName].GetLiveStreams(filterOptions, pageSize, s.PageToken);
            });

            return await Task.WhenAll(tasks);
        }

        public Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string streamingPlatformName, string streamerName)
        {
            var streamProvider = streamProviders[streamingPlatformName];

            return streamProvider.GetStreamerChannel(streamerName);
        }
    }
}
