using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamProviders.Dto.Twitch.Kraken;

namespace GameStreamSearch.StreamProviders.Mappers
{
    public class TwitchMapper
    {
        public Streams ToGameStreamsDto(IEnumerable<TwitchStreamDto> twitchStreams, NumericPageOffset nextPageOffset)
        {
            return new Streams(
                twitchStreams.Select(stream => new Stream
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
                nextPageOffset
            );
        }

        public PlatformChannel ToStreamerChannelDto(TwitchChannelsDto twitchChannels, string channelName)
        {
            return twitchChannels.Channels
                .Where(channel => channel.display_name.Equals(channelName, StringComparison.CurrentCultureIgnoreCase))
                .Select(channel => new PlatformChannel
                {
                    ChannelName = channel.display_name,
                    AvatarUrl = channel.logo,
                    ChannelUrl = channel.url,
                    Platform = StreamPlatformType.Twitch,
                })
                .FirstOrDefault();            
        }
    }
}
