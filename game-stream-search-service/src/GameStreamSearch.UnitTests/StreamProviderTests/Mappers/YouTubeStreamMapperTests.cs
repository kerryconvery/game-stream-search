﻿using System.Linq;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.ValueObjects;
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
                .GetOrElse(Streams.Empty);

            Assert.AreEqual(streams.Items.First().StreamerName, "test channel");
            Assert.AreEqual(streams.Items.First().StreamTitle, "test stream");
            Assert.AreEqual(streams.Items.First().StreamerAvatarUrl, "http://channel.thumbnail");
            Assert.AreEqual(streams.Items.First().StreamUrl, "http://youtube.com/watch?v=video1");
            Assert.AreEqual(streams.Items.First().StreamThumbnailUrl, "http://stream.thumbnail.url");
            Assert.AreEqual(streams.Items.First().IsLive, true);
            Assert.AreEqual(streams.Items.First().Views, 1);
            Assert.AreEqual(streams.Items.First().StreamPlatformName, StreamPlatformType.YouTube.GetFriendlyName());
            Assert.AreEqual(streams.NextPageToken, "nextPage");
        }

        [Test]
        public void Should_Return_An_Empty_List_Of_Streams_When_No_Streams_Where_Returned_From_The_Streaming_Platform()
        {
            var youTubeSearchResults = new YouTubeSearchResultsBuilder().Build();
            var videoDetails = new YouTubeVidoDetailResultsBuilder().Build();
            var videoChannels = new YouTubeChannelResultsBuilder().Build();

            var streams = new YouTubeStreamMapper(youTubeWebUrl)
                .Map(youTubeSearchResults, videoDetails, videoChannels)
                .GetOrElse(Streams.Empty);

            Assert.IsTrue(streams.IsEmpty());
        }
    }
}
