using System.Linq;
using GameStreamSearch.UnitTests.Builders;
using NUnit.Framework;
using System.Collections.Generic;
using GameStreamSearch.StreamProviders.Twitch.Gateways.Dto.Kraken;
using GameStreamSearch.StreamProviders.Twitch.Mappers;
using GameStreamSearch.StreamProviders.Const;

namespace GameStreamSearch.UnitTests.StreamProviders.Twitch.Mappers
{
    public class TwitchStreamMapperTests
    {
        private IEnumerable<TwitchStreamDto> twitchStreams;

        [SetUp]
        public void Setup()
        {
            twitchStreams = new TwitchStreamPreviewResultsBuilder()
                .Add("http://stream.thumbnail.url",
                    1,
                    "test channel",
                    "http://channel.logo.url",
                    "http://stream.url",
                    "test stream")
                .Build();
        }

        [Test]
        public void Should_Map_Twitch_Streams_To_Streams()
        {
            var platformStreams = new TwitchStreamMapper().Map(twitchStreams, 1, 0);

            Assert.AreEqual(platformStreams.Streams.First().StreamerName, "test channel");
            Assert.AreEqual(platformStreams.Streams.First().StreamerAvatarUrl, "http://channel.logo.url");
            Assert.AreEqual(platformStreams.Streams.First().StreamTitle, "test stream");
            Assert.AreEqual(platformStreams.Streams.First().StreamUrl, "http://stream.url");
            Assert.AreEqual(platformStreams.Streams.First().StreamThumbnailUrl, "http://stream.thumbnail.url");
            Assert.AreEqual(platformStreams.Streams.First().IsLive, true);
            Assert.AreEqual(platformStreams.Streams.First().Views, 1);
            Assert.AreEqual(platformStreams.StreamPlatformName, StreamPlatform.Twitch);
        }

        [Test]
        public void Should_Return_The_Next_Page_Token_When_The_Number_Of_Streams_Is_Equal_To_The_Page_Size()
        {
            var platformStreams = new TwitchStreamMapper().Map(twitchStreams, 1, 0);

            Assert.AreEqual(platformStreams.NextPageToken, "1");
        }

        [Test]
        public void Should_Return_An_Empty_Page_Token_When_The_Number_Of_Streams_Is_Less_Than_The_Page_Size()
        {
            var platformStreams = new TwitchStreamMapper().Map(twitchStreams, 2, 0);

            Assert.AreEqual(platformStreams.NextPageToken, string.Empty);
        }
    }
}
