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
        private IEnumerable<YouTubeChannelDto> videoChannels;

        public YouTubeStreamMapper(string youTubeWebUrl)
        {
            this.youTubeWebUrl = youTubeWebUrl;
        }

        public PlatformStreamsDto Map(
            YouTubeSearchDto searchResults,
            IEnumerable<YouTubeVideoDto> videos,
            IEnumerable<YouTubeChannelDto> videoChannels)
        {
            this.videoChannels = videoChannels;

            var liveStreamDetails = videos.ToDictionary(video => video.id, video => video.liveStreamingDetails);

            return new PlatformStreamsDto
            {
                StreamPlatformName = StreamPlatform.YouTube,
                Streams = searchResults.items.Select(v =>
                {
                    var viewers = liveStreamDetails.ContainsKey(v.id.videoId) ? liveStreamDetails[v.id.videoId].concurrentViewers : 0;
                    var maybeAvatarUrl = GetAvatarUrlOrDefault(v.snippet.channelId, string.Empty);

                    return new PlatformStreamDto
                    {
                        StreamerName = v.snippet.channelTitle,
                        StreamTitle = v.snippet.title,
                        StreamThumbnailUrl = v.snippet.thumbnails.medium.url,
                        StreamerAvatarUrl = maybeAvatarUrl,
                        StreamUrl = $"{youTubeWebUrl}/watch?v={v.id.videoId}",
                        IsLive = true,
                        Views = viewers,
                    };
                }),
                NextPageToken = searchResults.nextPageToken ?? string.Empty,
            };
        }

        private string GetAvatarUrlOrDefault(string videoChannelId, string defaultValue) {
            var avatarUrl = videoChannels
                .Where(channel => channel.id == videoChannelId)
                .Select(channel => channel.snippet.thumbnails.@default.url)
                .FirstOrDefault();

            return avatarUrl ?? defaultValue;
        }
    }
}
