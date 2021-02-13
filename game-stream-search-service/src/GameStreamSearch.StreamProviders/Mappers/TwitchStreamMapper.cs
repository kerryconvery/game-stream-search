using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamProviders.Dto.Twitch.Kraken;
using GameStreamSearch.Types;
using GameStreamSearch.Application;

namespace GameStreamSearch.StreamProviders.Mappers
{
    public class TwitchStreamMapper
    {
        public Streams Map(
            MaybeResult<IEnumerable<TwitchStreamDto>, StreamProviderError> twitchStreamResults,
            NumericPageOffset pageOffset)
        {
            return twitchStreamResults.Select(streams =>
            {
                return new Streams(
                    streams.Select(stream => new Stream
                    {
                        StreamTitle = stream.channel.status,
                        StreamerName = stream.channel.display_name,
                        StreamerAvatarUrl = stream.channel.logo,
                        StreamThumbnailUrl = stream.preview.medium,
                        StreamUrl = stream.channel.url,
                        StreamPlatformName = StreamPlatformType.Twitch.GetFriendlyName(),
                        IsLive = true,
                        Views = stream.viewers,
                    }),
                    pageOffset.GetNextOffset(streams.Count())
                );
            }).GetOrElse(Streams.Empty);
        }
    }
}
