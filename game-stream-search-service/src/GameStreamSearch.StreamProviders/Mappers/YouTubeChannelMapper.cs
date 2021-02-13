using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.StreamProviders.Dto.YouTube.YouTubeV3;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.Mappers
{
    public class YouTubeChannelMapper
    {
        private readonly string youTubeWebUrl;

        public YouTubeChannelMapper(string youTubeWebUrl)
        {
            this.youTubeWebUrl = youTubeWebUrl;
        }

        public MaybeResult<PlatformChannel, StreamProviderError> Map(
            MaybeResult<IEnumerable<YouTubeChannelDto>, StreamProviderError> channelSnippetResults)
        {
            return channelSnippetResults.Select(channelSnippets =>
            {
                return channelSnippets.Select(channelSnippet =>
                {
                    return new PlatformChannel
                    {
                        ChannelName = channelSnippet.snippet.title,
                        AvatarUrl = channelSnippet.snippet.thumbnails.@default.url,
                        ChannelUrl = $"{youTubeWebUrl}/user/{channelSnippet.snippet.title}",
                        Platform = StreamPlatformType.YouTube,
                    };
                })
                .FirstOrDefault();
            });
        }
    }
}
