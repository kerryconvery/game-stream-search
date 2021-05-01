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

        public async Task<IEnumerable<PlatformStreamsDto>> GetStreams(
            IEnumerable<string> streamPlatforms, StreamFilterOptions filterOptions, int pageSize, PageTokens pageTokens
        )
        {
            var tasks = streamPlatforms.Select(providerName =>
            {
                return streamProviderService.GetProviderByName(providerName).GetLiveStreams(filterOptions, pageSize, pageTokens.GetTokenOrEmpty(providerName));
            });

            return await Task.WhenAll(tasks);
        }

        public Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetPlatformChannel(string streamingPlatformName, string streamerName)
        {
            var streamProvider = streamProviderService.GetProviderByName(streamingPlatformName);

            return streamProvider.GetStreamerChannel(streamerName);
        }
    }
}
