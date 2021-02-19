using System;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.StreamProviders.Dto.Twitch.Kraken;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.Mappers
{
    public class TwitchChannelMapper
    {
        public MaybeResult<PlatformChannelDto, StreamProviderError> Map(
            string streamPlatformId,
            MaybeResult<TwitchChannelDto, StreamProviderError> twitchChannelResult)
        {
            return twitchChannelResult.Select(channel =>
            {
                return new PlatformChannelDto
                {
                    ChannelName = channel.display_name,
                    AvatarUrl = channel.logo,
                    ChannelUrl = channel.url,
                    StreamPlatformId = streamPlatformId,
                };
            });
        }
    }
}
