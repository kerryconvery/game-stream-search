using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using GameStreamSearch.Domain.Services;
using GameStreamSearch.Domain.Enums;
using GameStreamSearch.Application.Dto;

namespace GameStreamSearch.Domain.Tests
{
    public class StreaServiceTests
    {
        private Mock<IStreamProvider> CreateStreamProviderStub(StreamPlatformType streamingPlatform)
        {
            var streamProviderStub = new Mock<IStreamProvider>();

            streamProviderStub.SetupGet(m => m.Platform).Returns(streamingPlatform);

            return streamProviderStub;
        }

        [Test]
        public async Task Should_Return_An_Aggregated_List_Of_Streams_From_All_Registered_Providers()
        {
            var twitchStreamProviderStub = CreateStreamProviderStub(StreamPlatformType.twitch);
            var youtubeStreamProviderStub = CreateStreamProviderStub(StreamPlatformType.youtube);
            var streamFilterOptions = new StreamFilterOptionsDto();

            twitchStreamProviderStub.Setup(m => m.GetLiveStreams(streamFilterOptions, 2, null))
                .ReturnsAsync(new GameStreamsDto() { Items = new List<GameStreamDto>() { new GameStreamDto() } } );

            youtubeStreamProviderStub.Setup(m => m.GetLiveStreams(streamFilterOptions, 2, null))
                .ReturnsAsync(new GameStreamsDto() { Items = new List<GameStreamDto>() { new GameStreamDto() } } );

            var streamService = new StreamService()
                .RegisterStreamProvider(twitchStreamProviderStub.Object)
                .RegisterStreamProvider(youtubeStreamProviderStub.Object);

            var streams = await streamService.GetStreams(streamFilterOptions, 2, null);

            Assert.AreEqual(streams.Items.Count(), 2);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_When_No_Streams_Were_Found()
        {
            var twitchStreamProviderStub = CreateStreamProviderStub(StreamPlatformType.twitch);
            var streamFilterOptions = new StreamFilterOptionsDto();

            twitchStreamProviderStub.Setup(m => m.GetLiveStreams(streamFilterOptions, 1, null))
                .ReturnsAsync(GameStreamsDto.Empty());

            var streamService = new StreamService()
                .RegisterStreamProvider(twitchStreamProviderStub.Object);

            var streams = await streamService.GetStreams(streamFilterOptions, 1, null);

            Assert.AreEqual(streams.Items.Count(), 0);
        }

        [Test]
        public async Task Should_Pass_Page_Tokens_Where_A_Page_Token_For_The_Provider_Exists()
        {
            var twitchStreamProviderStub = CreateStreamProviderStub(StreamPlatformType.twitch);
            var youtubeStreamProviderStub = CreateStreamProviderStub(StreamPlatformType.youtube);
            var streamFilterOptions = new StreamFilterOptionsDto();

            twitchStreamProviderStub.Setup(m => m.GetLiveStreams(streamFilterOptions, It.IsAny<int>(), null))
                .ReturnsAsync(new GameStreamsDto()
                {
                    Items = new List<GameStreamDto>
                    {
                        new GameStreamDto
                        {
                            StreamTitle = "stream 1",
                            StreamPlatformName = twitchStreamProviderStub.Object.Platform.GetFriendlyName(),

                        }
                    },
                    NextPageToken = "twitch page token"
                });

            twitchStreamProviderStub.Setup(m => m.GetLiveStreams(streamFilterOptions, It.IsAny<int>(), "twitch page token"))
                .ReturnsAsync(new GameStreamsDto()
                {
                    Items = new List<GameStreamDto>
                    {
                        new GameStreamDto
                        {
                            StreamTitle = "stream 2",
                            StreamPlatformName = twitchStreamProviderStub.Object.Platform.GetFriendlyName(),
                        }
                    },
                });

            youtubeStreamProviderStub.Setup(m => m.GetLiveStreams(streamFilterOptions, It.IsAny<int>(), null))
                .ReturnsAsync(new GameStreamsDto()
                {
                    Items = new List<GameStreamDto>
                    {
                        new GameStreamDto
                        {
                            StreamPlatformName = youtubeStreamProviderStub.Object.Platform.GetFriendlyName(),
                        }
                    }
                });

            var streamService = new StreamService()
                .RegisterStreamProvider(twitchStreamProviderStub.Object)
                .RegisterStreamProvider(youtubeStreamProviderStub.Object);

            var firstPageOfStreams = await streamService.GetStreams(streamFilterOptions, 1, null);

            var nextPageOfStreams = await streamService.GetStreams(streamFilterOptions, 1, firstPageOfStreams.NextPageToken);

            Assert.AreEqual(nextPageOfStreams.Items.Count(), 2);
            Assert.AreEqual(nextPageOfStreams.Items.First().StreamPlatformName, StreamPlatformType.twitch.GetFriendlyName());
            Assert.AreEqual(nextPageOfStreams.Items.First().StreamTitle, "stream 2");
            Assert.AreEqual(nextPageOfStreams.Items.Last().StreamPlatformName, StreamPlatformType.youtube.GetFriendlyName());
        }

        [Test]
        public async Task Should_Return_A_Stream_List_Sorted_By_Views()
        {
            var twitchStreamProviderStub = CreateStreamProviderStub(StreamPlatformType.twitch);
            var streamFilterOptions = new StreamFilterOptionsDto();

            twitchStreamProviderStub.Setup(m => m.GetLiveStreams(streamFilterOptions, 2, null))
                .ReturnsAsync(new GameStreamsDto() { Items = new List<GameStreamDto>() {
                    new GameStreamDto() { Views = 1 },
                    new GameStreamDto() { Views = 2 }
                } });

            var streamService = new StreamService()
                .RegisterStreamProvider(twitchStreamProviderStub.Object);

            var streams = await streamService.GetStreams(streamFilterOptions, 2, null);

            Assert.AreEqual(streams.Items.First().Views, 2);
            Assert.AreEqual(streams.Items.Last().Views, 1);
        }
    }
}