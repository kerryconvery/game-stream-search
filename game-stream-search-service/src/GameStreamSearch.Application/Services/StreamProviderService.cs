using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Services
{
    public struct StreamSource
    {
        public string StreamPlatformId { get; init; }
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
            streamProviders.Add(streamProvider.StreamPlatformId, streamProvider);

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
                    StreamPlatformId = p.StreamPlatformId,
                    PageToken = pageTokens.GetValueOrDefault(p.StreamPlatformId, string.Empty)
                });
        }

        public async Task<IEnumerable<PlatformStreamsDto>> GetStreams(
            IEnumerable<StreamSource> sourcePlatforms, StreamFilterOptions filterOptions, int pageSize
        )
        {
            var tasks = sourcePlatforms.Select(s =>
            {
                return streamProviders[s.StreamPlatformId].GetLiveStreams(filterOptions, pageSize, s.PageToken);
            });

            return await Task.WhenAll(tasks);
        }

        public Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string streamerName, string streamingPlatformId)
        {
            var streamProvider = streamProviders[streamingPlatformId];

            return streamProvider.GetStreamerChannel(streamerName);
        }
    }
}
