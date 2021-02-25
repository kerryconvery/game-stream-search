using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Types;
using GameStreamSearch.Application.Models;
using System;
using GameStreamSearch.Application.Dto;

namespace GameStreamSearch.Application.Services
{
    public struct PlatformPageTokenPair
    {
        public string streamPlatformName { get; set; }
        public PageToken pageToken { get; set; }
    }

    public class StreamProviderService : IStreamService, IChannelService
    {
        private Dictionary<string, IStreamProvider> streamProviders;

        public StreamProviderService()
        {
            streamProviders = new Dictionary<string, IStreamProvider>(StringComparer.OrdinalIgnoreCase);
        }

        public StreamProviderService RegisterStreamProvider(IStreamProvider streamProvider)
        {
            streamProviders.Add(streamProvider.StreamPlatform.Name, streamProvider);

            return this;
        }

        public IEnumerable<string> GetSupportingPlatforms(StreamFilterOptions streamFilterOptions)
        {
            return streamProviders
                .Values
                .Where(p => p.AreFilterOptionsSupported(streamFilterOptions))
                .Select(p => p.StreamPlatform.Name);
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

        public Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string streamingPlatformName, string streamerName)
        {
            var streamProvider = streamProviders[streamingPlatformName];

            return streamProvider.GetStreamerChannel(streamerName);
        }
    }
}
