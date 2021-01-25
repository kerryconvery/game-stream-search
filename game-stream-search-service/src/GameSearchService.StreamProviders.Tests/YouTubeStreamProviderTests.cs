using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamProviders;
using GameStreamSearch.StreamProviders.Dto.YouTube.YouTubeV3;
using GameStreamSearch.Types;
using Moq;
using NUnit.Framework;

namespace GameSearchService.StreamProviders.Tests
{
    [TestFixture]
    public class YouTubeStreamProviderTests
    {
        private Mock<IYouTubeV3Api> youTubeV3ApiStub;

        [SetUp]
        public void Setup()
        {
            youTubeV3ApiStub = new Mock<IYouTubeV3Api>();
        }

        private YouTubeSearchDto liveStreams = new YouTubeSearchDto()
        {
            items = new List<YouTubeSearchItemDto>
             {
                 new YouTubeSearchItemDto
                 {
                     id = new YouTubeSearchItemIdDto
                     {
                         videoId = "stream1"
                     },
                     snippet = new YouTubeSearchSnippetDto
                     {
                         channelId = "channel1",
                         channelTitle = "fake channel",
                         title = "fake game",
                         thumbnails = new YouTubeSearchSnippetThumbnailsDto
                         {
                             medium = new YouTubeSearchSnippetThumbnailDto
                             {
                                 url = "http://fake.thumbnail.url"
                             }
                         }
                     }
                 }
             },
            nextPageToken = "next page token"
        };

        private IEnumerable<YouTubeVideoDto> videos = new List<YouTubeVideoDto>
        {
            new YouTubeVideoDto
            {
                id = "stream1",
                liveStreamingDetails = new YouTubeVideoLiveStreamingDetailsDto
                {
                    concurrentViewers = 5,
                }
            }
        };

        private IEnumerable<YouTubeChannelDto> channels = new List<YouTubeChannelDto>
        {
            new YouTubeChannelDto
            {
                id = "channel1",
                snippet = new YouTubeChannelSnippetDto
                {
                    thumbnails = new YouTubeChannelSnippetThumbnailsDto
                    {
                        @default = new YouTubeChannelSnippetThumbnailDto
                        {
                            url = "http://channel.thumpbnail.url",
                        }
                    }
                }
            }
        };

        private string youTubeBaseUrl = "fake.watch.url";

        [Test]
        public async Task Should_Return_A_List_Of_Streams()
        {
            youTubeV3ApiStub.Setup(m => m.SearchGamingVideos("fake game", VideoEventType.Live, VideoSortType.ViewCount, 1, "page token"))
                .ReturnsAsync(MaybeResult<YouTubeSearchDto, YouTubeErrorType>.Success(liveStreams));

            youTubeV3ApiStub.Setup(m => m.GetVideos(It.Is<string[]>(i => i.First() == "stream1")))
                .ReturnsAsync(MaybeResult<IEnumerable<YouTubeVideoDto>, YouTubeErrorType>.Success(videos));

            youTubeV3ApiStub.Setup(m => m.GetChannels(It.Is<string[]>(i => i.First() == "channel1")))
                .ReturnsAsync(MaybeResult<IEnumerable<YouTubeChannelDto>, YouTubeErrorType>.Success(channels));

            var youTubeStreamProvider = new YouTubeStreamProvider(youTubeBaseUrl, youTubeV3ApiStub.Object);

            var streams = await youTubeStreamProvider.GetLiveStreams(new StreamFilterOptions { GameName = "fake game" }, 1, "page token");

            Assert.AreEqual(streams.Items.Count(), 1);
            Assert.AreEqual(streams.Items.First().StreamTitle, liveStreams.items.First().snippet.title);
            Assert.AreEqual(streams.Items.First().StreamerName, liveStreams.items.First().snippet.channelTitle);
            Assert.AreEqual(streams.Items.First().StreamThumbnailUrl, liveStreams.items.First().snippet.thumbnails.medium.url);
            Assert.AreEqual(streams.Items.First().StreamerAvatarUrl, channels.First().snippet.thumbnails.@default.url);
            Assert.AreEqual(streams.Items.First().StreamUrl, $"{youTubeBaseUrl}/watch?v=stream1");
            Assert.AreEqual(streams.Items.First().StreamPlatformName, StreamPlatformType.YouTube.GetFriendlyName());
            Assert.AreEqual(streams.Items.First().Views, 5);
            Assert.IsTrue(streams.Items.First().IsLive);
            Assert.AreEqual(streams.NextPageToken, liveStreams.nextPageToken);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_Of_Streams_No_Streams_Were_Found()
        {
            youTubeV3ApiStub.Setup(m => m.SearchGamingVideos(null, VideoEventType.Live, VideoSortType.ViewCount, 1, null))
                .ReturnsAsync(MaybeResult<YouTubeSearchDto, YouTubeErrorType>.Success(new YouTubeSearchDto
                    {
                        items = new List<YouTubeSearchItemDto>()
                    }));

            var youTubeStreamProvider = new YouTubeStreamProvider(youTubeBaseUrl, youTubeV3ApiStub.Object);

            var streams = await youTubeStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1, null);

            Assert.IsEmpty(streams.Items);
            Assert.IsNull(streams.NextPageToken);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_Of_Streams_When_There_Is_An_Error_Getting_Streams()
        {
            youTubeV3ApiStub.Setup(m => m.SearchGamingVideos(null, VideoEventType.Live, VideoSortType.ViewCount, 1, null))
                .ReturnsAsync(MaybeResult<YouTubeSearchDto, YouTubeErrorType>.Fail(YouTubeErrorType.ProviderNotAvailable));

            var youTubeStreamProvider = new YouTubeStreamProvider(youTubeBaseUrl, youTubeV3ApiStub.Object);

            var streams = await youTubeStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1, null);

            Assert.IsEmpty(streams.Items);
            Assert.IsNull(streams.NextPageToken);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_Of_Streams_When_There_Is_An_Error_Getting_Stream_Channels()
        {
            youTubeV3ApiStub.Setup(m => m.GetChannels(It.IsAny<string[]>()))
                .ReturnsAsync(MaybeResult<IEnumerable<YouTubeChannelDto>, YouTubeErrorType>.Fail(YouTubeErrorType.ProviderNotAvailable));

            var youTubeStreamProvider = new YouTubeStreamProvider(youTubeBaseUrl, youTubeV3ApiStub.Object);

            var streams = await youTubeStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1, null);

            Assert.IsEmpty(streams.Items);
            Assert.IsNull(streams.NextPageToken);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_Of_Streams_When_There_Is_An_Error_Getting_Stream_Details()
        {
            youTubeV3ApiStub.Setup(m => m.GetVideos(It.IsAny<string[]>()))
                .ReturnsAsync(MaybeResult<IEnumerable<YouTubeVideoDto>, YouTubeErrorType>.Fail(YouTubeErrorType.ProviderNotAvailable));

            var youTubeStreamProvider = new YouTubeStreamProvider(youTubeBaseUrl, youTubeV3ApiStub.Object);

            var streams = await youTubeStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1, null);

            Assert.IsEmpty(streams.Items);
            Assert.IsNull(streams.NextPageToken);
        }

        [Test]
        public async Task Should_Return_Streamer_Channel_If_A_Channel_Was_Found_And_The_Name_Matched()
        {
            youTubeV3ApiStub.Setup(m => m.SearchChannelsByUsername("Test streamer", 1)).ReturnsAsync(
                MaybeResult<IEnumerable<YouTubeChannelDto>, YouTubeErrorType>.Success(
                    new List<YouTubeChannelDto>
                    {
                        {
                            new YouTubeChannelDto {
                                snippet = new YouTubeChannelSnippetDto
                                {
                                    title = "Test Streamer",
                                    thumbnails = new YouTubeChannelSnippetThumbnailsDto
                                    {
                                        @default = new YouTubeChannelSnippetThumbnailDto
                                        {
                                            url = "test.url"
                                        }
                                    }
                                }
                            }
                        }
                    })
                );

            var youTubeStreamProvider = new YouTubeStreamProvider(youTubeBaseUrl, youTubeV3ApiStub.Object);

            var streamerChannel = await youTubeStreamProvider.GetStreamerChannel("Test streamer");

            Assert.IsNotNull(streamerChannel.Value);
        }

        [Test]
        public async Task Should_Return_Nothing_If_A_Channel_Was_Not_Found()
        {
            youTubeV3ApiStub.Setup(m => m.SearchChannelsByUsername("Test streamer", 1)).ReturnsAsync(
                MaybeResult<IEnumerable<YouTubeChannelDto>, YouTubeErrorType>.Success(Maybe<IEnumerable<YouTubeChannelDto>>.Nothing)
            );

            var youTubeStreamProvider = new YouTubeStreamProvider(youTubeBaseUrl, youTubeV3ApiStub.Object);

            var streamerChannel = await youTubeStreamProvider.GetStreamerChannel("Test streamer");

            Assert.IsTrue(streamerChannel.Value.IsNothing);
        }

        [Test]
        public async Task Should_Return_Provider_Not_Available_If_The_Streaming_Platform_Service_Is_Unavailable()
        {
            youTubeV3ApiStub.Setup(m => m.SearchChannelsByUsername("Test streamer", 1)).ReturnsAsync(
                MaybeResult<IEnumerable<YouTubeChannelDto>, YouTubeErrorType>.Fail(YouTubeErrorType.ProviderNotAvailable)
            );

            var youTubeStreamProvider = new YouTubeStreamProvider(youTubeBaseUrl, youTubeV3ApiStub.Object);

            var streamerChannel = await youTubeStreamProvider.GetStreamerChannel("Test streamer");

            Assert.AreEqual(streamerChannel.Error, GetStreamerChannelErrorType.ProviderNotAvailable);
        }
    }
}
