using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.StreamProviders.Dto.Twitch.Kraken;
using GameStreamSearch.Types;
using GameStreamSearch.Application;

namespace GameStreamSearch.StreamProviders.Mappers
{
    public class TwitchStreamMapper
    {
        public PlatformStreamsDto Map(
            string streamPlatformId,
            MaybeResult<IEnumerable<TwitchStreamDto>, StreamProviderError> twitchStreamResults,
            int pageSize,
            int pageOffset
        )
        {
            return twitchStreamResults.Select(streams =>
            {
                return new PlatformStreamsDto
                {
                    StreamPlatformId = streamPlatformId,
                    Streams = streams.Select(stream => new PlatformStreamDto
                    {
                        StreamTitle = stream.channel.status,
                        StreamerName = stream.channel.display_name,
                        StreamerAvatarUrl = stream.channel.logo,
                        StreamThumbnailUrl = stream.preview.medium,
                        StreamUrl = stream.channel.url,
                        IsLive = true,
                        Views = stream.viewers,
                    }),
                    NextPageToken = streams.Count() == pageSize ? (pageOffset + pageSize).ToString() : string.Empty
                };
            }).GetOrElse(PlatformStreamsDto.Empty(streamPlatformId));
        }
    }
}
