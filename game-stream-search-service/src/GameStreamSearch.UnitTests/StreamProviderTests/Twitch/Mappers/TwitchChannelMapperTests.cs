using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.StreamProviders.Const;
using GameStreamSearch.StreamProviders.Twitch.Gateways.Dto.Kraken;
using GameStreamSearch.StreamProviders.Twitch.Mappers;
using GameStreamSearch.Types;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.StreamProviders.Twitch.Mappers
{
    public class TwitchChannelMapperTests
    {
        [Test]
        public void Should_Map_A_Twitch_Channel_To_A_PlatformChannel()
        {
            var twitchChannelDto = new TwitchChannelDto
            {
                display_name = "test channel",
                logo = "http://logo.url",
                url = "http://channel.url",
            };

            var platformChannel = new TwitchChannelMapper()
                .Map(twitchChannelDto);

            Assert.AreEqual(platformChannel.ChannelName, "test channel");
            Assert.AreEqual(platformChannel.AvatarUrl, "http://logo.url");
            Assert.AreEqual(platformChannel.ChannelUrl, "http://channel.url");
            Assert.AreEqual(platformChannel.StreamPlatformName, StreamPlatform.Twitch);
        }
    }
}
