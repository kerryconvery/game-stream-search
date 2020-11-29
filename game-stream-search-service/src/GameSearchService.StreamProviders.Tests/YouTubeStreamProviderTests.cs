using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Exceptions;
using GameStreamSearch.StreamPlatformApi;
using GameStreamSearch.StreamPlatformApi.YouTube.Dto.YouTubeV3;
using GameStreamSearch.StreamProviders;
using GameStreamSearch.StreamProviders.Builders;
using Moq;
using NUnit.Framework;

namespace GameSearchService.StreamProviders.Tests
{
    
    public class YouTubeStreamProviderTests
    {
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

        private YouTubeVideosDto videos = new YouTubeVideosDto
        {
            items = new List<YouTubeVideoDto>
            {
                new YouTubeVideoDto
                {
                    id = "stream1",
                    liveStreamingDetails = new YouTubeVideoLiveStreamingDetailsDto
                    {
                        concurrentViewers = 5,
                    }
                }
            }
        };

        private YouTubeChannelsDto channels = new YouTubeChannelsDto
        {
            items = new List<YouTubeChannelDto>
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
            }
        };

        private Mock<IYouTubeWatchUrlBuilder> watchUrlBuilderStub;
        private Mock<IYouTubeChannelUrlBuilder> channelUrlBuilderStub;

        private string watchUrl = "fake.watch.url";

        [SetUp]
        public void Setup()
        {
            watchUrlBuilderStub = new Mock<IYouTubeWatchUrlBuilder>();
            watchUrlBuilderStub.Setup(m => m.Build("stream1")).Returns(watchUrl);

            channelUrlBuilderStub = new Mock<IYouTubeChannelUrlBuilder>();
            channelUrlBuilderStub.Setup(s => s.Build("channel1"));
        }

        [Test]
        public async Task Should_Return_A_List_Of_Streams()
        {
            var youTubeV3ApiStub = new Mock<IYouTubeV3Api>();

            youTubeV3ApiStub.Setup(m => m.SearchGamingVideos("fake game", VideoEventType.Live, VideoSortType.ViewCount, 1, "page token")).ReturnsAsync(liveStreams);
            youTubeV3ApiStub.Setup(m => m.GetVideos(It.Is<string[]>(i => i.First() == "stream1"))).ReturnsAsync(videos);
            youTubeV3ApiStub.Setup(m => m.GetChannels(It.Is<string[]>(i => i.First() == "channel1"))).ReturnsAsync(channels);

            var youTubeStreamProvider = new YouTubeStreamProvider(watchUrlBuilderStub.Object, channelUrlBuilderStub.Object, youTubeV3ApiStub.Object);

            var streams = await youTubeStreamProvider.GetLiveStreams(new StreamFilterOptionsDto { GameName = "fake game" }, 1, "page token");

            Assert.AreEqual(streams.Items.Count(), 1);
            Assert.AreEqual(streams.Items.First().StreamTitle, liveStreams.items.First().snippet.title);
            Assert.AreEqual(streams.Items.First().StreamerName, liveStreams.items.First().snippet.channelTitle);
            Assert.AreEqual(streams.Items.First().StreamThumbnailUrl, liveStreams.items.First().snippet.thumbnails.medium.url);
            Assert.AreEqual(streams.Items.First().StreamerAvatarUrl, channels.items.First().snippet.thumbnails.@default.url);
            Assert.AreEqual(streams.Items.First().StreamUrl, watchUrl);
            Assert.AreEqual(streams.Items.First().StreamPlatformName, StreamPlatformType.youtube.GetFriendlyName());
            Assert.AreEqual(streams.Items.First().Views, 5);
            Assert.IsTrue(streams.Items.First().IsLive);
            Assert.AreEqual(streams.NextPageToken, liveStreams.nextPageToken);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_Of_Streams_No_Streams_Were_Found()
        {
            var youTubeV3ApiStub = new Mock<IYouTubeV3Api>();

            youTubeV3ApiStub.Setup(m => m.SearchGamingVideos(null, VideoEventType.Live, VideoSortType.ViewCount, 1, null)).ReturnsAsync(
                new YouTubeSearchDto
                {
                    items = new List<YouTubeSearchItemDto>()
                }); ;

            var youTubeStreamProvider = new YouTubeStreamProvider(watchUrlBuilderStub.Object, channelUrlBuilderStub.Object, youTubeV3ApiStub.Object);

            var streams = await youTubeStreamProvider.GetLiveStreams(new StreamFilterOptionsDto(), 1, null);

            Assert.IsEmpty(streams.Items);
            Assert.IsNull(streams.NextPageToken);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_Of_Streams_When_There_Is_An_Api_Error()
        {
            var youTubeV3ApiStub = new Mock<IYouTubeV3Api>();

            youTubeV3ApiStub.Setup(m => m.SearchGamingVideos(null, VideoEventType.Live, VideoSortType.ViewCount, 1, null)).ReturnsAsync(new YouTubeSearchDto());

            var youTubeStreamProvider = new YouTubeStreamProvider(watchUrlBuilderStub.Object, channelUrlBuilderStub.Object, youTubeV3ApiStub.Object);

            var streams = await youTubeStreamProvider.GetLiveStreams(new StreamFilterOptionsDto(), 1, null);

            Assert.IsEmpty(streams.Items);
            Assert.IsNull(streams.NextPageToken);
        }

        [Test]
        public async Task Should_Return_Streamer_Channel_If_A_Channel_Was_Found_And_The_Name_Matched()
        {
            var youTubeV3ApiStub = new Mock<IYouTubeV3Api>();

            youTubeV3ApiStub.Setup(m => m.SearchChannelsByUsername("Test streamer", 1)).ReturnsAsync(
                new YouTubeChannelsDto
                {
                    items = new List<YouTubeChannelDto> {
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
                }
            );

            var youTubeStreamProvider = new YouTubeStreamProvider(watchUrlBuilderStub.Object, channelUrlBuilderStub.Object, youTubeV3ApiStub.Object);

            var streamerChannel = await youTubeStreamProvider.GetStreamerChannel("Test streamer");

            Assert.IsNotNull(streamerChannel);
        }

        [Test]
        public async Task Should_Return_Null_If_A_Channel_Was_Found_But_The_Name_Does_Not_Match()
        {
            var youTubeV3ApiStub = new Mock<IYouTubeV3Api>();

            youTubeV3ApiStub.Setup(m => m.SearchChannelsByUsername("Test streamer", 1)).ReturnsAsync(
                new YouTubeChannelsDto
                {
                    items = new List<YouTubeChannelDto> {
                        new YouTubeChannelDto {
                            snippet = new YouTubeChannelSnippetDto
                            {
                                title = "Test Streamer Two",
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
                }
            );

            var youTubeStreamProvider = new YouTubeStreamProvider(watchUrlBuilderStub.Object, channelUrlBuilderStub.Object, youTubeV3ApiStub.Object);

            var streamerChannel = await youTubeStreamProvider.GetStreamerChannel("Test streamer");

            Assert.IsNull(streamerChannel);
        }

        [Test]
        public async Task Should_Return_Null_If_A_Channel_Was_Not_Found()
        {
            var youTubeV3ApiStub = new Mock<IYouTubeV3Api>();

            youTubeV3ApiStub.Setup(m => m.SearchChannelsByUsername("Test streamer", 1)).ReturnsAsync(
                new YouTubeChannelsDto
                {
                    items = new List<YouTubeChannelDto>()
                }
            );

            var youTubeStreamProvider = new YouTubeStreamProvider( watchUrlBuilderStub.Object, channelUrlBuilderStub.Object, youTubeV3ApiStub.Object);

            var streamerChannel = await youTubeStreamProvider.GetStreamerChannel("Test streamer");

            Assert.IsNull(streamerChannel);
        }
    }
}
