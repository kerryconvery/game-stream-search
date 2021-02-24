using System;
using System.Collections.Generic;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto.YouTube.YouTubeV3;
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

        public MaybeResult<IEnumerable<YouTubeVideoDto>, StreamProviderError> Build()
        {
            return MaybeResult<IEnumerable<YouTubeVideoDto>, StreamProviderError>.Success(youTubeVideoDtos);
        }
    }
}
