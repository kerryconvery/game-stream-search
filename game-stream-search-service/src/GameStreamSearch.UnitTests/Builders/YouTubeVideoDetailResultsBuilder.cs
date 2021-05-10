using System.Collections.Generic;
using GameStreamSearch.StreamProviders.YouTube.Gateways.Dto.V3;
using GameStreamSearch.Types;

namespace GameStreamSearch.UnitTests.Builders
{
    internal class YouTubeVidoDetailResultsBuilder
    {
        private List<YouTubeVideoDto> youTubeVideoDtos = new List<YouTubeVideoDto>();

        public YouTubeVidoDetailResultsBuilder Add(string videoId, int concurrentViewers)
        {
            youTubeVideoDtos.Add(new YouTubeVideoDto
            {
                id = videoId,
                liveStreamingDetails = new YouTubeVideoLiveStreamingDetailsDto { concurrentViewers = concurrentViewers }
            });

            return this;
        }

        public IEnumerable<YouTubeVideoDto> Build()
        {
            return youTubeVideoDtos;
        }
    }
}
