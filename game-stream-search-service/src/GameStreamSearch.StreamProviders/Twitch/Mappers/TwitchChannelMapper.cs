using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.StreamProviders.Const;
using GameStreamSearch.StreamProviders.Twitch.Gateways.Dto.Kraken;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.Twitch.Mappers
{
    public class TwitchChannelMapper
    {
        public Maybe<PlatformChannelDto> Map(
            Maybe<TwitchChannelDto> maybeChannel)
        {
            return maybeChannel.Select(channel =>
            {
                return new PlatformChannelDto
                {
                    ChannelName = channel.display_name,
                    AvatarUrl = channel.logo,
                    ChannelUrl = channel.url,
                    StreamPlatformName = StreamPlatform.Twitch,
                };
            });
        }
    }
}
