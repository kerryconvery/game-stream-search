using System.Linq;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.StreamProviders.Mappers;
using GameStreamSearch.UnitTests.Builders;
using GameStreamSearch.UnitTests.Extensions;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.StreamProviders.Mappers
{
    public class TwitchStreamMapperTests
    {
        [Test]
        public void Should_Map_Twitch_Streams_To_Streams()
        {
            var twitchStreamResults = new TwitchStreamPreviewResultsBuilder()
                .Add("http://stream.thumbnail.url",
                1,
                "test channel",
                "http://channel.logo.url",
                "http://stream.url",
                "test stream")
                .Build();

            var streams = new TwitchStreamMapper().Map(twitchStreamResults, new NumericPageOffset(1, "0"));

            Assert.AreEqual(streams.Items.First().StreamerName, "test channel");
            Assert.AreEqual(streams.Items.First().StreamerAvatarUrl, "http://channel.logo.url");
            Assert.AreEqual(streams.Items.First().StreamTitle, "test stream");
            Assert.AreEqual(streams.Items.First().StreamUrl, "http://stream.url");
            Assert.AreEqual(streams.Items.First().StreamThumbnailUrl, "http://stream.thumbnail.url");
            Assert.AreEqual(streams.Items.First().IsLive, true);
            Assert.AreEqual(streams.Items.First().Views, 1);
            Assert.AreEqual(streams.Items.First().StreamPlatformName, StreamPlatformType.Twitch.GetFriendlyName());
            Assert.AreEqual(streams.NextPageToken, "1");
        }

        [Test]
        public void Should_Return_An_Empty_List_Of_Streams_When_No_Streams_Where_Returned_From_The_Streaming_Platform()
        {
            var emptySearchResults = new TwitchStreamPreviewResultsBuilder().Build();

            var streams = new TwitchStreamMapper().Map(emptySearchResults, new NumericPageOffset(1, "0"));

            Assert.IsTrue(streams.IsEmpty());
            Assert.IsEmpty(streams.NextPageToken);
        }
    }
}
