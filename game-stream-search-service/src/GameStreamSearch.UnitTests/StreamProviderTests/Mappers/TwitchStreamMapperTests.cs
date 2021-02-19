using System.Linq;
using GameStreamSearch.StreamProviders.Mappers;
using GameStreamSearch.UnitTests.Builders;
using GameStreamSearch.UnitTests.Extensions;
using NUnit.Framework;
using GameStreamSearch.Types;
using System.Collections.Generic;
using GameStreamSearch.StreamProviders.Dto.Twitch.Kraken;
using GameStreamSearch.Application;

namespace GameStreamSearch.UnitTests.StreamProviders.Mappers
{
    public class TwitchStreamMapperTests
    {
        private string streamPlatformId = "twitch";
        private MaybeResult<IEnumerable<TwitchStreamDto>, StreamProviderError> twitchStreamResults;

        [SetUp]
        public void Setup()
        {
            twitchStreamResults = new TwitchStreamPreviewResultsBuilder()
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
            var platformStreams = new TwitchStreamMapper().Map(streamPlatformId, twitchStreamResults, 1, 0);

            Assert.AreEqual(platformStreams.Streams.First().StreamerName, "test channel");
            Assert.AreEqual(platformStreams.Streams.First().StreamerAvatarUrl, "http://channel.logo.url");
            Assert.AreEqual(platformStreams.Streams.First().StreamTitle, "test stream");
            Assert.AreEqual(platformStreams.Streams.First().StreamUrl, "http://stream.url");
            Assert.AreEqual(platformStreams.Streams.First().StreamThumbnailUrl, "http://stream.thumbnail.url");
            Assert.AreEqual(platformStreams.Streams.First().IsLive, true);
            Assert.AreEqual(platformStreams.Streams.First().Views, 1);
            Assert.AreEqual(platformStreams.StreamPlatformId, streamPlatformId);
        }

        [Test]
        public void Should_Return_The_Next_Page_Token_When_The_Number_Of_Streams_Is_Equal_To_The_Page_Size()
        {
            var platformStreams = new TwitchStreamMapper().Map(streamPlatformId, twitchStreamResults, 1, 0);

            Assert.AreEqual(platformStreams.NextPageToken, "1");
        }

        [Test]
        public void Should_Return_An_Empty_Page_Token_When_The_Number_Of_Streams_Is_Less_Than_The_Page_Size()
        {
            var platformStreams = new TwitchStreamMapper().Map(streamPlatformId, twitchStreamResults, 2, 0);

            Assert.AreEqual(platformStreams.NextPageToken, string.Empty);
        }

        [Test]
        public void Should_Return_An_Empty_List_Of_Streams_When_No_Streams_Where_Returned_From_The_Streaming_Platform()
        {
            var emptySearchResults = new TwitchStreamPreviewResultsBuilder().Build();

            var streams = new TwitchStreamMapper().Map(streamPlatformId, emptySearchResults, 1, 0);

            Assert.IsTrue(streams.IsEmpty());
            Assert.IsEmpty(streams.NextPageToken);
        }
    }
}
