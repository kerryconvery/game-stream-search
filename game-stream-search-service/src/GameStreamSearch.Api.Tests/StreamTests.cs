using System.Linq;
using GameStreamSearch.Api.Controllers;
using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using GameStreamSearch.Application.Services;
using GameStreamSearch.StreamProviders.ProviderApi.Twitch.Dto.Kraken;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Interfaces;
using GameStreamSearch.StreamProviders.ProviderApi.Twitch.Interfaces;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Dto.YouTubeV3;
using Microsoft.AspNetCore.Mvc;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application;

namespace GameStreamSearch.Api.Tests
{
    public class StreamTests
    {
        private StreamController streamController;
        private Mock<IStreamProvider> twitchStreamProviderStub;
        private Mock<IStreamProvider> youTubeStreamProviderStub;

        private Mock<IStreamProvider> CreateStreamProviderStub(StreamingPlatform streamingPlatform)
        {
            var streamProviderStub = new Mock<IStreamProvider>();

            streamProviderStub.SetupGet(m => m.Platform).Returns(streamingPlatform);

            return streamProviderStub;
        }

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

            twitchStreamProviderStub = CreateStreamProviderStub(StreamingPlatform.twitch);
            youTubeStreamProviderStub = CreateStreamProviderStub(StreamingPlatform.youtube);

            var streamService = new StreamService()
                .RegisterStreamProvider(twitchStreamProviderStub.Object)
                .RegisterStreamProvider(youTubeStreamProviderStub.Object);

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
            var twitchStreams = new GameStreamsDto
            {
                Items = new List<GameStreamDto>
                {
                    new GameStreamDto
                    {
                        StreamTitle = "game 1",
                        StreamerName = "streamer 1",
                        StreamUrl = "http://fake.twitch.url",
                        StreamerAvatarUrl = "http://channel.thumbnail.url",
                        StreamThumbnailUrl = "http://twitch.thumbnail.url",
                        StreamPlatformName = twitchStreamProviderStub.Object.Platform.GetFriendlyName(),
                        IsLive = true,
                        Views = 1,
                    }
                },
                NextPageToken = "page token 1"
            };


            var youtubeStreams = new GameStreamsDto
            {
                Items = new List<GameStreamDto>
                {
                    new GameStreamDto()
                    {
                        StreamTitle = "game 2",
                        StreamerName = "streamer 2",
                        StreamUrl = "https://www.youtube.com/watch?v=video1",
                        StreamerAvatarUrl = "http://channel1.thumbnail.url",
                        StreamThumbnailUrl = "http://youtube.thumbnail",
                        StreamPlatformName= youTubeStreamProviderStub.Object.Platform.GetFriendlyName(),
                        IsLive = true,
                        Views = 1,
                    }
                },
                NextPageToken = "page token 2"
            };

            twitchStreamProviderStub.Setup(s => s.GetLiveStreams(It.IsAny<StreamFilterOptionsDto>(), 1, null)).ReturnsAsync(twitchStreams);
            youTubeStreamProviderStub.Setup(s => s.GetLiveStreams(It.IsAny<StreamFilterOptionsDto>(), 1, null)).ReturnsAsync(youtubeStreams);

            var response = await streamController.GetStreams(null, 1);
            var streams = (response as OkObjectResult).Value as GameStreamsDto;

            Assert.AreEqual(streams.Items.Count() , 2);

            Assert.AreEqual(streams.Items.First().StreamTitle, twitchStreams.Items.First().StreamTitle);
            Assert.AreEqual(streams.Items.First().StreamPlatformName, twitchStreams.Items.First().StreamPlatformName);
            Assert.AreEqual(streams.Items.First().StreamerName, twitchStreams.Items.First().StreamerName);
            Assert.AreEqual(streams.Items.First().StreamUrl, twitchStreams.Items.First().StreamUrl);
            Assert.AreEqual(streams.Items.First().StreamerAvatarUrl, twitchStreams.Items.First().StreamerAvatarUrl);
            Assert.AreEqual(streams.Items.First().StreamThumbnailUrl, twitchStreams.Items.First().StreamThumbnailUrl);
            Assert.AreEqual(streams.Items.First().IsLive, twitchStreams.Items.First().IsLive);
            Assert.AreEqual(streams.Items.First().Views, twitchStreams.Items.First().Views);

            Assert.AreEqual(streams.Items.Last().StreamTitle, youtubeStreams.Items.First().StreamTitle);
            Assert.AreEqual(streams.Items.Last().StreamPlatformName, youtubeStreams.Items.First().StreamPlatformName);
            Assert.AreEqual(streams.Items.Last().StreamerName, youtubeStreams.Items.First().StreamerName);
            Assert.AreEqual(streams.Items.Last().StreamUrl, youtubeStreams.Items.First().StreamUrl);
            Assert.AreEqual(streams.Items.Last().StreamerAvatarUrl, youtubeStreams.Items.First().StreamerAvatarUrl);
            Assert.AreEqual(streams.Items.Last().StreamThumbnailUrl, youtubeStreams.Items.First().StreamThumbnailUrl);
            Assert.AreEqual(streams.Items.Last().IsLive, youtubeStreams.Items.First().IsLive);
            Assert.AreEqual(streams.Items.Last().Views, youtubeStreams.Items.First().Views);

            Assert.IsNotNull(streams.NextPageToken);
        }

        [Test]
        public async Task Should_Return_All_Paged_Streams_Filtered_By_Game_Name()
        {
            var firstPageResponse = await streamController.GetStreams("game 2", 1);
            var firstPageStreams = (firstPageResponse as OkObjectResult).Value as GameStreamsDto;

            var secondPageResponse = await streamController.GetStreams("game 2", 1, firstPageStreams.NextPageToken);
            var secondPageStreams = (secondPageResponse as OkObjectResult).Value as GameStreamsDto;

            var thirdPageResponse = await streamController.GetStreams("game 2", 1, secondPageStreams.NextPageToken);
            var thirdPageStreams = (thirdPageResponse as OkObjectResult).Value as GameStreamsDto;

            Assert.AreEqual(secondPageStreams.Items.Count(), 2);

            Assert.AreEqual(secondPageStreams.Items.First().StreamTitle, "game 2");
            Assert.AreEqual(secondPageStreams.Items.First().StreamPlatformName, StreamingPlatform.twitch.GetFriendlyName());

            Assert.AreEqual(secondPageStreams.Items.Last().StreamTitle, "game 2");
            Assert.AreEqual(secondPageStreams.Items.Last().StreamPlatformName, StreamingPlatform.youtube.GetFriendlyName());
            Assert.AreEqual(secondPageStreams.Items.Last().Views, 2);

            Assert.IsNull(thirdPageStreams.NextPageToken);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_Of_Unfiltered_Streams_When_There_Is_An_Error_Fetching_The_Streams()
        {
            var response = await streamController.GetStreams("error", 1);
            var streams = (response as OkObjectResult).Value as GameStreamsDto;

            Assert.AreEqual(streams.Items.Count(), 0);
        }
    }
}