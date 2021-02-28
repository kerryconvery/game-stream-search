using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Types;
using System;
using GameStreamSearch.Application.StreamProvider;
using GameStreamSearch.Application.StreamProvider.Dto;

namespace GameStreamSearch.Application.Services.StreamProvider
{
    public class StreamPlatformService
    {
        private Dictionary<string, IStreamProvider> streamProviders;

        public StreamPlatformService()
        {
            streamProviders = new Dictionary<string, IStreamProvider>(StringComparer.OrdinalIgnoreCase);
        }

        public StreamPlatformService RegisterStreamProvider(IStreamProvider streamProvider)
        {
            streamProviders.Add(streamProvider.StreamPlatformName, streamProvider);

            return this;
        }

        public IEnumerable<string> GetSupportingPlatforms(StreamFilterOptions streamFilterOptions)
        {
            return streamProviders
                .Values
                .Where(p => p.AreFilterOptionsSupported(streamFilterOptions))
                .Select(p => p.StreamPlatformName);
        }

        public async Task<IEnumerable<PlatformStreamsDto>> GetStreams(
            IEnumerable<string> streamPlatforms, StreamFilterOptions filterOptions, int pageSize, PageTokens pageTokens
        )
        {
            var tasks = streamPlatforms.Select(p =>
            {
                return streamProviders[p].GetLiveStreams(filterOptions, pageSize, pageTokens.GetTokenOrEmpty(p));
            });

            return await Task.WhenAll(tasks);
        }

        public Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetPlatformChannel(string streamingPlatformName, string streamerName)
        {
            var streamProvider = streamProviders[streamingPlatformName];

            return streamProvider.GetStreamerChannel(streamerName);
        }
    }
}
