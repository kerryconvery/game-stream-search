using System.Collections.Generic;
using GameStreamSearch.Gateways.Dto.Twitch.Kraken;
using GameStreamSearch.StreamProviders.Selectors;
using GameStreamSearch.Types;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.StreamProviders.Selectors
{
    public class TwitchChannelSelectorTests
    {
        private TwitchChannelDto expectedChannel = new TwitchChannelDto { display_name = "expected channel" };
        private TwitchChannelDto anotherChannel = new TwitchChannelDto { display_name = "Another channel" };

        [Test]
        public void Should_Return_Back_The_Matching_Channel()
        {
            var channels = new List<TwitchChannelDto> { expectedChannel, anotherChannel };
            var channelResults = MaybeResult<IEnumerable<TwitchChannelDto>, StreamProviderError>.Success(channels);

            var channel = TwitchChannelSelector.Select(expectedChannel.display_name, channelResults);

            Assert.AreEqual(channel.GetOrElse(new TwitchChannelDto()), expectedChannel);
        }

        [Test]
        public void Should_Return_Back_Nothing_When_No_Matching_Channel_Was_Found()
        {
            var channels = new List<TwitchChannelDto> { anotherChannel };
            var channelResults = MaybeResult<IEnumerable<TwitchChannelDto>, StreamProviderError>.Success(channels);
           
            var channel = TwitchChannelSelector.Select(expectedChannel.display_name, channelResults);

            Assert.IsTrue(channel.IsNothing);
        }
    }
}
