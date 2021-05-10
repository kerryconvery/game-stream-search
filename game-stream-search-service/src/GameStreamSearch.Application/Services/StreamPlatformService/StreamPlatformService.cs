using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Types;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.Common;

namespace GameStreamSearch.Application.Services.StreamProvider
{
    public class StreamPlatformService
    {
        private readonly StreamProviderService streamProviderService;

        public StreamPlatformService(StreamProviderService streamProviderService)
        {
            this.streamProviderService = streamProviderService;
        }

        public async Task<IEnumerable<PlatformStreamsDto>> GetStreams(StreamFilterOptions filterOptions, int pageSize, PageTokens pageTokens)
        {
            var tasks = streamProviderService.StreamProviders.Select(provider =>
            {
                return provider.GetLiveStreams(filterOptions, pageSize, pageTokens.GetTokenOrEmpty(provider.StreamPlatformName));
            });

            return await Task.WhenAll(tasks);
        }

        public Task<Maybe<PlatformChannelDto>> GetPlatformChannel(string streamingPlatformName, string streamerName)
        {
            var streamProvider = streamProviderService.GetProviderByName(streamingPlatformName);

            return streamProvider.GetStreamerChannel(streamerName);
        }
    }
}
