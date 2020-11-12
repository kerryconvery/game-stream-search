using System.Linq;
using GameStreamSearch.Api.Infrastructor;
using GameStreamSearch.Providers;
using GameStreamSearch.Services;
using GameStreamSearch.Api.Controllers;
using NUnit.Framework;
using Moq;
using GameStreamSearch.StreamProviders.ProviderApi.Twitch.Interfaces;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Interfaces;
using GameStreamSearch.StreamProviders;
using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.ProviderApi.Twitch.Dto.Kraken;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Dto.YouTubeV3;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using GameStreamSearch.StreamProviders.Builders;

namespace GameStreamSearch.Api.Tests
{
    public class StreamTests
    {
        private StreamController streamController;

        private StreamController SetupStreamData(
            List<TwitchLiveStreamDto> twitchLiveStreamDataPages,
            List<YouTubeSearchDto> youTubLiveStreamDataPages,
            YouTubeVideosDto videos,
            YouTubeChannelsDto channels)
        {
            var twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();
            twitchKrakenApiStub.Setup(m => m.GetLiveStreams(1, 0)).ReturnsAsync(twitchLiveStreamDataPages[0]);
            twitchKrakenApiStub.Setup(m => m.SearchStreams("game 2", 1, 0)).ReturnsAsync(twitchLiveStreamDataPages[1]);
            twitchKrakenApiStub.Setup(m => m.SearchStreams("game 2", 1, 1)).ReturnsAsync(twitchLiveStreamDataPages[2]);
            twitchKrakenApiStub.Setup(m => m.SearchStreams("game 2", 1, 2)).ReturnsAsync(twitchLiveStreamDataPages[3]);

            twitchKrakenApiStub.Setup(m => m.SearchStreams("error", 1, 0)).ReturnsAsync(new TwitchLiveStreamDto());

            var youTubeV3ApiStub = new Mock<IYouTubeV3Api>();
            youTubeV3ApiStub.Setup(m => m.SearchGamingVideos(null, VideoEventType.Live, VideoSortType.ViewCount, 1, null)).ReturnsAsync(youTubLiveStreamDataPages[0]);
            youTubeV3ApiStub.Setup(m => m.SearchGamingVideos("game 2", VideoEventType.Live, VideoSortType.ViewCount, 1, null)).ReturnsAsync(youTubLiveStreamDataPages[1]);
            youTubeV3ApiStub.Setup(m => m.SearchGamingVideos("game 2", VideoEventType.Live, VideoSortType.ViewCount, 1, "page.token.2")).ReturnsAsync(youTubLiveStreamDataPages[2]);
            youTubeV3ApiStub.Setup(m => m.SearchGamingVideos("game 2", VideoEventType.Live, VideoSortType.ViewCount, 1, "page.token.3")).ReturnsAsync(youTubLiveStreamDataPages[3]);
             
            youTubeV3ApiStub.Setup(m => m.SearchGamingVideos("error", VideoEventType.Live, VideoSortType.ViewCount, 1, null)).ReturnsAsync(new YouTubeSearchDto());
            youTubeV3ApiStub.Setup(m => m.GetVideos(It.IsAny<string[]>())).ReturnsAsync(videos);
            youTubeV3ApiStub.Setup(m => m.GetChannels(It.IsAny<string[]>())).ReturnsAsync(channels);

            var youTubeStreamUrl = "https://www.youtube.com";

            var twitchStreamProvider = new TwitchStreamProvider("Twitch", twitchKrakenApiStub.Object);
            var youTubeStreamProvider = new YouTubeStreamProvider("YouTube", new YouTubeWatchUrlBuilder(youTubeStreamUrl), youTubeV3ApiStub.Object);

            var paginator = new Paginator();

            var streamService = new StreamService(paginator)
                .RegisterStreamProvider(twitchStreamProvider)
                .RegisterStreamProvider(youTubeStreamProvider);

            return new StreamController(streamService);
        }

        private T LoadTestData<T>(string fileName)
        {
            string testPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return JsonConvert.DeserializeObject<T>(File.ReadAllText($"{testPath}/TestData/{fileName}"));
        }

        [SetUp]
        public void Setup()
        {
            var twitchLiveStreamData = LoadTestData<List<TwitchLiveStreamDto>>("TwitchLiveStreams.json");
            var youTubeLiveStreamData = LoadTestData<List<YouTubeSearchDto>>("YouTubeLiveStreams.json");
            var youTubeVideos = LoadTestData<YouTubeVideosDto>("YouTubeVideos.json");
            var youTubeChannels = LoadTestData<YouTubeChannelsDto>("YouTubeChannels.json");

            streamController = SetupStreamData(twitchLiveStreamData, youTubeLiveStreamData, youTubeVideos, youTubeChannels);
        }

        [Test]
        public async Task Should_Return_A_Paged_List_Of_Unfiltered_Live_Streams_From_All_Providers()
        {
            var streams = await streamController.GetStreams(null, 1);

            Assert.AreEqual(streams.Value.Items.Count() , 2);

            Assert.AreEqual(streams.Value.Items.First().StreamTitle, "game 1");
            Assert.AreEqual(streams.Value.Items.First().PlatformName, "Twitch");
            Assert.AreEqual(streams.Value.Items.First().StreamerName, "twitch channel 1");
            Assert.AreEqual(streams.Value.Items.First().StreamUrl, "http://fake.twitch.url");
            Assert.AreEqual(streams.Value.Items.First().StreamerAvatarUrl, "http://channel.thumbnail.url");
            Assert.AreEqual(streams.Value.Items.First().StreamThumbnailUrl, "http://twitch.thumbnail.url");
            Assert.AreEqual(streams.Value.Items.First().IsLive, true);
            Assert.AreEqual(streams.Value.Items.First().Views, 1);


            Assert.AreEqual(streams.Value.Items.Last().StreamTitle, "game 1");
            Assert.AreEqual(streams.Value.Items.Last().PlatformName, "YouTube");
            Assert.AreEqual(streams.Value.Items.Last().StreamerName, "youtube channel 1");
            Assert.AreEqual(streams.Value.Items.Last().StreamUrl, "https://www.youtube.com/watch?v=video1");
            Assert.AreEqual(streams.Value.Items.Last().StreamerAvatarUrl, "http://channel1.thumbnail.url");
            Assert.AreEqual(streams.Value.Items.Last().StreamThumbnailUrl, "http://youtube.thumbnail");
            Assert.AreEqual(streams.Value.Items.Last().IsLive, true);

            Assert.AreEqual(streams.Value.Items.Last().Views, 1);

            Assert.IsNotNull(streams.Value.NextPageToken);
        }

        [Test]
        public async Task Should_Return_All_Paged_Streams_Filtered_By_Game_Name()
        {
            var firstPageStreams = await streamController.GetStreams("game 2", 1);
            var secondPageStreams = await streamController.GetStreams("game 2", 1, firstPageStreams.Value.NextPageToken);
            var thirdPageStreams = await streamController.GetStreams("game 2", 1, secondPageStreams.Value.NextPageToken);

            Assert.AreEqual(secondPageStreams.Value.Items.Count(), 2);

            Assert.AreEqual(secondPageStreams.Value.Items.First().StreamTitle, "game 2");
            Assert.AreEqual(secondPageStreams.Value.Items.First().PlatformName, "Twitch");

            Assert.AreEqual(secondPageStreams.Value.Items.Last().StreamTitle, "game 2");
            Assert.AreEqual(secondPageStreams.Value.Items.Last().PlatformName, "YouTube");
            Assert.AreEqual(secondPageStreams.Value.Items.Last().Views, 2);

            Assert.IsNull(thirdPageStreams.Value.NextPageToken);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_Of_Unfiltered_Streams_When_There_Is_An_Error_Fetching_The_Streams()
        {
            var streams = await streamController.GetStreams("error", 1);

            Assert.AreEqual(streams.Value.Items.Count(), 0);
        }
    }
}