using GameStreamSearch.Application.Mappers;
using GameStreamSearch.Types;
using NUnit.Framework;
using GameStreamSearch.Gateways.Dto.Twitch.Kraken;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Domain.Entities;

namespace GameStreamSearch.UnitTests.StreamProviders.Mappers
{
    public class TwitchChannelMapperTests
    {
        [Test]
        public void Should_Map_The_Channel_Exactly_Matching_The_Channel_Name_To_PlatformChannel()
        {
            var twitchChannelDto = new TwitchChannelDto
            {
                display_name = "test channel",
                logo = "http://logo.url",
                url = "http://channel.url",
            };
            var twitchChannelResult = MaybeResult<TwitchChannelDto, StreamProviderError>.Success(twitchChannelDto);

            var platformChannel = new TwitchChannelMapper().Map(twitchChannelResult).GetOrElse(new PlatformChannelDto());

            Assert.AreEqual(platformChannel.ChannelName, "test channel");
            Assert.AreEqual(platformChannel.AvatarUrl, "http://logo.url");
            Assert.AreEqual(platformChannel.ChannelUrl, "http://channel.url");
            Assert.AreEqual(platformChannel.StreamPlatformName, StreamPlatform.Twitch.Name);
        }
    }
}
