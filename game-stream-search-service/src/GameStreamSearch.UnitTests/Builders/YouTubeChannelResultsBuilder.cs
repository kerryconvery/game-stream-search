using System;
using System.Collections.Generic;
using GameStreamSearch.Application;
using GameStreamSearch.StreamProviders.Dto.YouTube.YouTubeV3;
using GameStreamSearch.Types;

namespace GameStreamSearch.UnitTests.Builders
{
    internal class YouTubeChannelResultsBuilder
    {
        private List<YouTubeChannelDto> channelDtos = new List<YouTubeChannelDto>();

        public YouTubeChannelResultsBuilder Add(string id, string channelThumbnailUrl)
        {
            var youTubeChannelDto = new YouTubeChannelDto
            {
                id = id,
                snippet = new YouTubeChannelSnippetDto
                {
                    thumbnails = new YouTubeChannelSnippetThumbnailsDto
                    {
                        @default = new YouTubeChannelSnippetThumbnailDto
                        {
                            url = channelThumbnailUrl,
                        }
                    }
                }
            };

            channelDtos.Add(youTubeChannelDto);
            return this;
        }

        public MaybeResult<IEnumerable<YouTubeChannelDto>, StreamProviderError> Build()
        {
            return MaybeResult<IEnumerable<YouTubeChannelDto>, StreamProviderError>.Success(channelDtos);
        }
    }
}
