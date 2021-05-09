using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.StreamProviders.Const;
using GameStreamSearch.StreamProviders.YouTube.Gateways.Dto.V3;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.YouTube.Mappers.V3
{
    public class YouTubeStreamMapper
    {
        private readonly string youTubeWebUrl;

        public YouTubeStreamMapper(string youTubeWebUrl)
        {
            this.youTubeWebUrl = youTubeWebUrl;
        }

        public PlatformStreamsDto Map(
            YouTubeSearchDto searchResults,
            IEnumerable<YouTubeVideoDto> videos,
            IEnumerable<YouTubeChannelDto> channels)
        {
            var liveStreamDetails = videos.ToDictionary(video => video.id, video => video.liveStreamingDetails);
            var channelSnippets = channels.ToDictionary(channel => channel.id, channel => channel.snippet);

            return new PlatformStreamsDto
            {
                StreamPlatformName = StreamPlatform.YouTube,
                Streams = searchResults.items.Select(v =>
                {
                    var viewers = liveStreamDetails.ContainsKey(v.id.videoId) ? liveStreamDetails[v.id.videoId].concurrentViewers : 0;
                    var avatarUrl = channelSnippets.ContainsKey(v.snippet.channelId) ? channelSnippets[v.snippet.channelId].thumbnails.@default.url : null;

                    return new PlatformStreamDto
                    {
                        StreamerName = v.snippet.channelTitle,
                        StreamTitle = v.snippet.title,
                        StreamThumbnailUrl = v.snippet.thumbnails.medium.url,
                        StreamerAvatarUrl = avatarUrl,
                        StreamUrl = $"{youTubeWebUrl}/watch?v={v.id.videoId}",
                        IsLive = true,
                        Views = viewers,
                    };
                }),
                NextPageToken = searchResults.nextPageToken ?? string.Empty,
            };
        }
    }
}
