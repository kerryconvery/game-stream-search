using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.StreamProviders.Dto.YouTube.YouTubeV3;

namespace GameStreamSearch.StreamProviders.Mappers
{
    public class YouTubeMapper
    {
        private readonly string youTubeWebUrl;

        public YouTubeMapper(string youTubeWebUrl)
        {
            this.youTubeWebUrl = youTubeWebUrl;
        }

        public Streams ToPlatformStreams(
            YouTubeSearchDto streams,
            Dictionary<string, YouTubeChannelSnippetDto> channelSnippets,
            Dictionary<string, YouTubeVideoLiveStreamingDetailsDto> liveStreamDetails)
        {
            return new Streams(
                streams.items.Select(v =>
                {
                    var viewers = liveStreamDetails.ContainsKey(v.id.videoId) ? liveStreamDetails[v.id.videoId].concurrentViewers : 0;
                    var avatarUrl = channelSnippets.ContainsKey(v.snippet.channelId) ? channelSnippets[v.snippet.channelId].thumbnails.@default.url : null;

                    return new Stream
                    {
                        StreamerName = v.snippet.channelTitle,
                        StreamTitle = v.snippet.title,
                        StreamThumbnailUrl = v.snippet.thumbnails.medium.url,
                        StreamerAvatarUrl = avatarUrl,
                        StreamUrl = $"{youTubeWebUrl}/watch?v={v.id.videoId}",
                        StreamPlatformName = StreamPlatformType.YouTube.GetFriendlyName(),
                        IsLive = true,
                        Views = viewers,
                    };
                }),
                streams.nextPageToken ?? string.Empty
            );
        }

        public PlatformChannel ToPlatformChannel(YouTubeChannelSnippetDto channelSnippet)
        {
            return new PlatformChannel
            {
                ChannelName = channelSnippet.title,
                AvatarUrl = channelSnippet.thumbnails.@default.url,
                ChannelUrl = $"{youTubeWebUrl}/user/{channelSnippet.title}",
                Platform = StreamPlatformType.YouTube,
            };
        }
    }
}
