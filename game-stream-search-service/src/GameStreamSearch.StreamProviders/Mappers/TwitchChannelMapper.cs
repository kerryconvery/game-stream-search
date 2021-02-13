using System;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.StreamProviders.Dto.Twitch.Kraken;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.Mappers
{
    public class TwitchChannelMapper
    {
        public MaybeResult<PlatformChannel, StreamProviderError> Map(
            MaybeResult<TwitchChannelDto, StreamProviderError> twitchChannelResult)
        {
            return twitchChannelResult.Select(channel =>
            {
                return new PlatformChannel
                {
                    ChannelName = channel.display_name,
                    AvatarUrl = channel.logo,
                    ChannelUrl = channel.url,
                    Platform = StreamPlatformType.Twitch,
                };
            });
        }
    }
}
