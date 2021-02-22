using System.Linq;
using GameStreamSearch.Application.Models;
using GameStreamSearch.StreamProviders.Mappers;
using GameStreamSearch.UnitTests.Builders;
using GameStreamSearch.UnitTests.Extensions;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.StreamProviders.Mappers
{
    public class YouTubeStreamMapperTests
    {
        private string youTubeWebUrl = "http://youtube.com";

        [Test]
        public void Should_Map_YouTube_Streams_To_Streams()
        {
            var youTubeSearchResults = new YouTubeSearchResultsBuilder()
                .Add("video1", "test stream", "test channel", "channel1", "http://stream.thumbnail.url")
                .SetNextPageToken("nextPage")
                .Build();

            var videoDetails = new YouTubeVidoDetailResultsBuilder()
                .Add("video1", 1)
                .Build();

            var videoChannels = new YouTubeChannelResultsBuilder()
                .Add("channel1", "http://channel.thumbnail")
                .Build();

            var streams = new YouTubeStreamMapper(youTubeWebUrl)
                .Map(youTubeSearchResults, videoDetails, videoChannels)
                .GetOrElse(PlatformStreamsDto.Empty(StreamPlatform.YouTube));

            Assert.AreEqual(streams.Streams.First().StreamerName, "test channel");
            Assert.AreEqual(streams.Streams.First().StreamTitle, "test stream");
            Assert.AreEqual(streams.Streams.First().StreamerAvatarUrl, "http://channel.thumbnail");
            Assert.AreEqual(streams.Streams.First().StreamUrl, "http://youtube.com/watch?v=video1");
            Assert.AreEqual(streams.Streams.First().StreamThumbnailUrl, "http://stream.thumbnail.url");
            Assert.AreEqual(streams.Streams.First().IsLive, true);
            Assert.AreEqual(streams.Streams.First().Views, 1);
            Assert.AreEqual(streams.NextPageToken, "nextPage");
            Assert.AreEqual(streams.StreamPlatformName, StreamPlatform.YouTube.Name);

        }

        [Test]
        public void Should_Return_An_Empty_List_Of_Streams_When_No_Streams_Where_Returned_From_The_Streaming_Platform()
        {
            var youTubeSearchResults = new YouTubeSearchResultsBuilder().Build();
            var videoDetails = new YouTubeVidoDetailResultsBuilder().Build();
            var videoChannels = new YouTubeChannelResultsBuilder().Build();

            var streams = new YouTubeStreamMapper(youTubeWebUrl)
                .Map(youTubeSearchResults, videoDetails, videoChannels)
                .GetOrElse(PlatformStreamsDto.Empty(StreamPlatform.YouTube.Name));

            Assert.IsTrue(streams.IsEmpty());
        }
    }
}
