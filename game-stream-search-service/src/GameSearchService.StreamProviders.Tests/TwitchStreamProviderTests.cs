using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using GameStreamSearch.StreamProviders;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamPlatformApi.Twitch.Dto.Kraken;
using GameStreamSearch.StreamPlatformApi;
using GameStreamSearch.Application;

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

            var twitchStreamProvider = new TwitchStreamProvider(twitchKrakenApiStub.Object);

            var streams = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1);

            Assert.AreEqual(streams.Items.Count(), 1);
            Assert.AreEqual(streams.Items.First().StreamTitle, liveStreams.streams.First().channel.status);
            Assert.AreEqual(streams.Items.First().StreamerName, liveStreams.streams.First().channel.display_name);
            Assert.AreEqual(streams.Items.First().StreamerAvatarUrl, liveStreams.streams.First().channel.logo);
            Assert.AreEqual(streams.Items.First().StreamPlatformName, StreamPlatformType.Twitch.GetFriendlyName());
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

            var twitchStreamProvider = new TwitchStreamProvider(twitchKrakenApiStub.Object);

            var streams = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptions { GameName = "fake game" }, 1);

            Assert.AreEqual(streams.Items.Count(), 1);
            Assert.AreEqual(streams.Items.First().StreamTitle, liveStreams.streams.First().channel.status);
            Assert.AreEqual(streams.Items.First().StreamerName, liveStreams.streams.First().channel.display_name);
            Assert.AreEqual(streams.Items.First().StreamerAvatarUrl, liveStreams.streams.First().channel.logo);
            Assert.AreEqual(streams.Items.First().StreamPlatformName, StreamPlatformType.Twitch.GetFriendlyName());
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

            var twitchStreamProvider = new TwitchStreamProvider(twitchKrakenApiStub.Object);

            var streams = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1);

            Assert.IsEmpty(streams.Items);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_Of_Filtered_Streams_When_The_Api_Call_Fails()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.SearchStreams("fake game", 1, 0)).ReturnsAsync(new TwitchLiveStreamDto());

            var twitchStreamProvider = new TwitchStreamProvider(twitchKrakenApiStub.Object);

            var streams = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptions { GameName = "fake game" }, 1);

            Assert.IsEmpty(streams.Items);
        }

        [Test]
        public async Task Should_Return_The_Second_Page_Of_Unfiltered_Streams()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.GetLiveStreams(1, 0)).ReturnsAsync(liveStreams);
            twitchKrakenApiStub.Setup(m => m.GetLiveStreams(1, 1)).ReturnsAsync(liveStreamsB);

            var twitchStreamProvider = new TwitchStreamProvider(twitchKrakenApiStub.Object);

            var streamsPage1 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1);
            var streamsPage2 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1, streamsPage1.NextPageToken);

            Assert.AreEqual(streamsPage2.Items.Count(), 1);
            Assert.AreEqual(streamsPage2.Items.First().StreamerName, liveStreamsB.streams.First().channel.display_name);
        }

        [Test]
        public async Task Should_Return_The_Second_Page_Of_Filtered_Streams()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.SearchStreams("fake game", 1, 0)).ReturnsAsync(liveStreams);
            twitchKrakenApiStub.Setup(m => m.SearchStreams("fake game", 1, 1)).ReturnsAsync(liveStreamsB);

            var twitchStreamProvider = new TwitchStreamProvider(twitchKrakenApiStub.Object);

            var streamsPage1 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptions() { GameName = "fake game" }, 1);
            var streamsPage2 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptions() { GameName = "fake game" }, 1, streamsPage1.NextPageToken);

            Assert.AreEqual(streamsPage2.Items.Count(), 1);
            Assert.AreEqual(streamsPage2.Items.First().StreamerName, liveStreamsB.streams.First().channel.display_name);
        }

        [Test]
        public async Task Should_Return_A_Null_Next_Page_Token_When_There_Are_No_More_Unfiltered_Streams()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.GetLiveStreams(1, 0)).ReturnsAsync(liveStreams);
            twitchKrakenApiStub.Setup(m => m.GetLiveStreams(1, 1)).ReturnsAsync(new TwitchLiveStreamDto { streams = new List<TwitchStreamDto>() });

            var twitchStreamProvider = new TwitchStreamProvider(twitchKrakenApiStub.Object);

            var streamsPage1 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1);
            var streamsPage2 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1, streamsPage1.NextPageToken);

            Assert.IsNull(streamsPage2.NextPageToken);
        }

        [Test]
        public async Task Should_Return_A_Null_Next_Page_Token_When_There_Are_No_More_Filtered_Streams()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.SearchStreams("fake game", 1, 0)).ReturnsAsync(liveStreams);
            twitchKrakenApiStub.Setup(m => m.SearchStreams("fake game", 1, 1)).ReturnsAsync(new TwitchLiveStreamDto { streams = new List<TwitchStreamDto>() });

            var twitchStreamProvider = new TwitchStreamProvider(twitchKrakenApiStub.Object);

            var streamsPage1 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptions() { GameName = "fake game" }, 1);
            var streamsPage2 = await twitchStreamProvider.GetLiveStreams(new StreamFilterOptions() { GameName = "fake game" }, 1, streamsPage1.NextPageToken);

            Assert.IsNull(streamsPage2.NextPageToken);
        }

        [Test]
        public async Task Should_Return_Streamer_Channel_If_A_Channel_Was_Found_And_The_Name_Matched()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.SearchChannels("Test streamer", 1, 0)).ReturnsAsync(
                new TwitchChannelsDto
                {
                    Channels = new List<TwitchChannelDto> { new TwitchChannelDto { display_name = "Test Streamer" } }
                }
            );

            var twitchStreamProvider = new TwitchStreamProvider(twitchKrakenApiStub.Object);

            var streamerChannel = await twitchStreamProvider.GetStreamerChannel("Test streamer");

            Assert.IsNotNull(streamerChannel);
        }

        [Test]
        public async Task Should_Return_Nothing_If_A_Channel_Was_Found_But_The_Name_Does_Not_Match()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.SearchChannels("Test streamer", 1, 0)).ReturnsAsync(
                new TwitchChannelsDto
                {
                    Channels = new List<TwitchChannelDto> { new TwitchChannelDto { display_name = "Test Streamer Two" } }
                }
            );

            var twitchStreamProvider = new TwitchStreamProvider(twitchKrakenApiStub.Object);

            var streamerChannel = await twitchStreamProvider.GetStreamerChannel("Test streamer");

            Assert.IsTrue(streamerChannel.Value.IsNothing);
        }

        [Test]
        public async Task Should_Return_Nothing_If_A_Channel_Was_Not_Found()
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();

            twitchKrakenApiStub.Setup(m => m.SearchChannels("Test streamer", 1, 0)).ReturnsAsync(
                new TwitchChannelsDto
                {
                    Channels = new List<TwitchChannelDto>()
                }
            );

            var twitchStreamProvider = new TwitchStreamProvider(twitchKrakenApiStub.Object);

            var streamerChannel = await twitchStreamProvider.GetStreamerChannel("Test streamer");

            Assert.IsTrue(streamerChannel.Value.IsNothing);
        }
    }
}