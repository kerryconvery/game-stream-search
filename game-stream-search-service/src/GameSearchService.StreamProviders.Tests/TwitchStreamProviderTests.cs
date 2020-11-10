using NUnit.Framework;
using Moq;
using GameStreamSearch.StreamProviders.ProviderApi.Twitch.Interfaces;
using GameStreamSearch.Application.Dto;
using System.Threading.Tasks;
using System.Linq;
using GameStreamSearch.StreamProviders.ProviderApi.Twitch.Dto.Kraken;
using System.Collections.Generic;
using GameStreamSearch.StreamProviders;

namespace GameSearchService.StreamProviders.Tests
{
    public class TwitchStreamProviderTests
    {
        private TwitchLiveStreamDto liveStreams = new TwitchLiveStreamDto
        {
            streams = new List<TwitchStreamDto>
            {
                new TwitchStreamDto
                {
                    channel = new TwitchChannelDto
                    {
                        status = "fake game",
                        display_name = "fake channel",
                        logo = "http://stream.logo.url",
                        url = "http://fake.stream.url",
                    },
                    preview = new TwitchStreamPreviewDto
                    {
                        large = "http://fake.thumbnail.url"
                    },
                    viewers = 1,
                }
            }
        };

        private TwitchLiveStreamDto liveStreamsB = new TwitchLiveStreamDto
        {
            streams = new List<TwitchStreamDto>
            {
                new TwitchStreamDto
                {
                    channel = new TwitchChannelDto
                    {
                        status = "fake game",
                        display_name = "fake channel B",
                        logo = "http://stream.logo.url",
                        url = "http://fake.stream.url",
                    },
                    preview = new TwitchStreamPreviewDto
                    {
                        large = "http://fake.thumbnail.url"
                    },
                    viewers = 1,
                }
            }
        };

        [Test]
        public async Task Should_Return_Live_Streams_When_Not_Filtering()
        {

            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();
            twitchKrakenApiStub.Setup(m => m.GetLiveStreams(1, 0)).ReturnsAsync(liveStreams);

            var twitchStreamProvider = new TwitchStreamProvider("Twitch", twitchKrakenApiStub.Object);

            var streams = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptionsDto(), 1);

            Assert.AreEqual(streams.Items.Count(), 1);
            Assert.AreEqual(streams.Items.First().StreamTitle, liveStreams.streams.First().channel.status);
            Assert.AreEqual(streams.Items.First().StreamerName, liveStreams.streams.First().channel.display_name);
            Assert.AreEqual(streams.Items.First().StreamerAvatarUrl, liveStreams.streams.First().channel.logo);
            Assert.AreEqual(streams.Items.First().PlatformName, "Twitch");
            Assert.AreEqual(streams.Items.First().StreamThumbnailUrl, liveStreams.streams.First().preview.medium);
            Assert.AreEqual(streams.Items.First().StreamUrl, liveStreams.streams.First().channel.url);
            Assert.AreEqual(streams.Items.First().Views, liveStreams.streams.First().viewers);
            Assert.AreEqual(streams.Items.First().IsLive, true);
        }

        [Test]
        public async Task Should_Return_Filtered_Live_Streams()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.SearchStreams("fake game", 1, 0)).ReturnsAsync(liveStreams);

            var twitchStreamProvider = new TwitchStreamProvider("Twitch", twitchKrakenApiStub.Object);

            var streams = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptionsDto { GameName = "fake game" }, 1);

            Assert.AreEqual(streams.Items.Count(), 1);
            Assert.AreEqual(streams.Items.First().StreamTitle, liveStreams.streams.First().channel.status);
            Assert.AreEqual(streams.Items.First().StreamerName, liveStreams.streams.First().channel.display_name);
            Assert.AreEqual(streams.Items.First().StreamerAvatarUrl, liveStreams.streams.First().channel.logo);
            Assert.AreEqual(streams.Items.First().PlatformName, "Twitch");
            Assert.AreEqual(streams.Items.First().StreamThumbnailUrl, liveStreams.streams.First().preview.medium);
            Assert.AreEqual(streams.Items.First().StreamUrl, liveStreams.streams.First().channel.url);
            Assert.AreEqual(streams.Items.First().Views, liveStreams.streams.First().viewers);
            Assert.AreEqual(streams.Items.First().IsLive, true);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_Of_Unfiltered_Streams_When_The_Api_Call_Fails()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.GetLiveStreams(1, 0)).ReturnsAsync(new TwitchLiveStreamDto());

            var twitchStreamProvider = new TwitchStreamProvider("Twitch", twitchKrakenApiStub.Object);

            var streams = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptionsDto(), 1);

            Assert.IsEmpty(streams.Items);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_Of_Filtered_Streams_When_The_Api_Call_Fails()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.SearchStreams("fake game", 1, 0)).ReturnsAsync(new TwitchLiveStreamDto());

            var twitchStreamProvider = new TwitchStreamProvider("Twitch", twitchKrakenApiStub.Object);

            var streams = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptionsDto { GameName = "fake game" }, 1);

            Assert.IsEmpty(streams.Items);
        }

        [Test]
        public async Task Should_Return_The_Second_Page_Of_Unfiltered_Streams()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.GetLiveStreams(1, 0)).ReturnsAsync(liveStreams);
            twitchKrakenApiStub.Setup(m => m.GetLiveStreams(1, 1)).ReturnsAsync(liveStreamsB);

            var twitchStreamProvider = new TwitchStreamProvider("Twitch", twitchKrakenApiStub.Object);

            var streamsPage1 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptionsDto(), 1);
            var streamsPage2 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptionsDto(), 1, streamsPage1.NextPageToken);

            Assert.AreEqual(streamsPage2.Items.Count(), 1);
            Assert.AreEqual(streamsPage2.Items.First().StreamerName, liveStreamsB.streams.First().channel.display_name);
        }

        [Test]
        public async Task Should_Return_The_Second_Page_Of_Filtered_Streams()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.SearchStreams("fake game", 1, 0)).ReturnsAsync(liveStreams);
            twitchKrakenApiStub.Setup(m => m.SearchStreams("fake game", 1, 1)).ReturnsAsync(liveStreamsB);

            var twitchStreamProvider = new TwitchStreamProvider("Twitch", twitchKrakenApiStub.Object);

            var streamsPage1 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptionsDto() { GameName = "fake game" }, 1);
            var streamsPage2 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptionsDto() { GameName = "fake game" }, 1, streamsPage1.NextPageToken);

            Assert.AreEqual(streamsPage2.Items.Count(), 1);
            Assert.AreEqual(streamsPage2.Items.First().StreamerName, liveStreamsB.streams.First().channel.display_name);
        }

        [Test]
        public async Task Should_Return_A_Null_Next_Page_Token_When_There_Are_No_More_Unfiltered_Streams()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.GetLiveStreams(1, 0)).ReturnsAsync(liveStreams);
            twitchKrakenApiStub.Setup(m => m.GetLiveStreams(1, 1)).ReturnsAsync(new TwitchLiveStreamDto { streams = new List<TwitchStreamDto>() });

            var twitchStreamProvider = new TwitchStreamProvider("Twitch", twitchKrakenApiStub.Object);

            var streamsPage1 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptionsDto(), 1);
            var streamsPage2 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptionsDto(), 1, streamsPage1.NextPageToken);

            Assert.IsNull(streamsPage2.NextPageToken);
        }

        [Test]
        public async Task Should_Return_A_Null_Next_Page_Token_When_There_Are_No_More_Filtered_Streams()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.SearchStreams("fake game", 1, 0)).ReturnsAsync(liveStreams);
            twitchKrakenApiStub.Setup(m => m.SearchStreams("fake game", 1, 1)).ReturnsAsync(new TwitchLiveStreamDto { streams = new List<TwitchStreamDto>() });

            var twitchStreamProvider = new TwitchStreamProvider("Twitch", twitchKrakenApiStub.Object);

            var streamsPage1 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptionsDto() { GameName = "fake game" }, 1);
            var streamsPage2 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptionsDto() { GameName = "fake game" }, 1, streamsPage1.NextPageToken);

            Assert.IsNull(streamsPage2.NextPageToken);
        }
    }
}