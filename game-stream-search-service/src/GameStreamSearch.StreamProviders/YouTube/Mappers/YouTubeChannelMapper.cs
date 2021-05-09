using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.StreamProviders.Const;
using GameStreamSearch.StreamProviders.YouTube.Gateways.Dto.V3;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.YouTube.Mappers.V3
{
    public class YouTubeChannelMapper
    {
        private readonly string youTubeWebUrl;

        public YouTubeChannelMapper(string youTubeWebUrl)
        {
            this.youTubeWebUrl = youTubeWebUrl;
        }

        public PlatformChannelDto Map(YouTubeChannelDto channel)
        {
            return new PlatformChannelDto
            {
                ChannelName = channel.snippet.title,
                AvatarUrl = channel.snippet.thumbnails.@default.url,
                ChannelUrl = $"{youTubeWebUrl}/channel/{channel.id}",
                StreamPlatformName = StreamPlatform.YouTube,
            };
        }
    }
}
