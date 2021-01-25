using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamPlatformApi;
using GameStreamSearch.StreamPlatformApi.DLive.Dto;
using GameStreamSearch.Types;
using Moq;
using NUnit.Framework;

namespace GameStreamSearch.StreamProviders.Tests
{
    [TestFixture]
    public class DLiveStreamProviderTests
    {
        private Mock<IDLiveApi> dliveApiStub;

        [SetUp]
        public void Setup()
        {
            dliveApiStub = new Mock<IDLiveApi>();
        }

        [Test]
        public async Task Should_Return_Streams_And_A_Next_Page_Token_When_There_Are_No_Filters_Set()
        {
            var streamWatchUrl = "http://stream.watch.url";

            var dliveStreams = new DLiveStreamDto
            {
                data = new DLiveStreamDataDto
                {
                    livestreams = new DLiveStreamsDto
                    {
                        list = new List<DLiveStreamItemDto>
                        {
                            new DLiveStreamItemDto
                            {
                                title = "fake dlive stream",
                                thumbnailUrl = "http://fake.dlive.thumbnail",
                                watchingCount = 100,
                                creator = new DLiveUserDto
                                {
                                    displayName = "FakeStreamer",
                                    avatar = "http://fake.avatar.url"
                                }
                            }
                        }
                    }
                }
            };

            dliveApiStub.Setup(m => m.GetLiveStreams(1, 0, StreamSortOrder.Trending))
                .ReturnsAsync(MaybeResult<DLiveStreamDto, DLiveErrorType>.Success(dliveStreams));

            var dliveStreamProvider = new DLiveStreamProvider(streamWatchUrl, dliveApiStub.Object);

            var streams = await dliveStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1, string.Empty);

            Assert.AreEqual(streams.Items.Count(), 1);
            Assert.NotNull(streams.NextPageToken);
            Assert.AreEqual(streams.Items.First().StreamTitle, dliveStreams.data.livestreams.list.First().title);
            Assert.AreEqual(streams.Items.First().StreamUrl, $"{streamWatchUrl}/FakeStreamer");
            Assert.AreEqual(streams.Items.First().StreamerAvatarUrl, dliveStreams.data.livestreams.list.First().creator.avatar);
            Assert.AreEqual(streams.Items.First().StreamThumbnailUrl, dliveStreams.data.livestreams.list.First().thumbnailUrl);
            Assert.AreEqual(streams.Items.First().StreamerName, dliveStreams.data.livestreams.list.First().creator.displayName);
            Assert.AreEqual(streams.Items.First().Views, dliveStreams.data.livestreams.list.First().watchingCount);
            Assert.AreEqual(streams.Items.First().StreamPlatformName, StreamPlatformType.DLive.GetFriendlyName());
            Assert.AreEqual(streams.Items.First().IsLive, true);
        }


        [Test]
        public async Task Should_Return_No_Streams_When_A_Filter_Is_Applied()
        {
            var dliveStreamProvider = new DLiveStreamProvider("", dliveApiStub.Object);

            var streams = await dliveStreamProvider.GetLiveStreams(new StreamFilterOptions { GameName = "some game" }, 1, string.Empty);

            Assert.False(streams.Items.Any());
            Assert.IsNull(streams.NextPageToken);
        }

        [Test]
        public async Task Should_Return_The_Next_Page_Of_Streams()
        {
            var dliveStreamsPage1 = new DLiveStreamDto
            {
                data = new DLiveStreamDataDto
                {
                    livestreams = new DLiveStreamsDto
                    {
                        list = new List<DLiveStreamItemDto>
                        {
                            new DLiveStreamItemDto
                            {
                                title = "fake dlive stream page 1",
                                thumbnailUrl = "http://fake.dlive.thumbnail",
                                watchingCount = 100,
                                creator = new DLiveUserDto
                                {
                                    displayName = "FakeStreamer",
                                    avatar = "http://fake.avatar.url"
                                }
                            }
                        }
                    }
                }
            };

            var dliveStreamsPage2 = new DLiveStreamDto
            {
                data = new DLiveStreamDataDto
                {
                    livestreams = new DLiveStreamsDto
                    {
                        list = new List<DLiveStreamItemDto>
                        {
                            new DLiveStreamItemDto
                            {
                                title = "fake dlive stream page 2",
                                thumbnailUrl = "http://fake.dlive.thumbnail",
                                watchingCount = 100,
                                creator = new DLiveUserDto
                                {
                                    displayName = "FakeStreamer",
                                    avatar = "http://fake.avatar.url"
                                }
                            }
                        }
                    }
                }
            };

            dliveApiStub.Setup(m => m.GetLiveStreams(1, 0, StreamSortOrder.Trending))
                .ReturnsAsync(MaybeResult<DLiveStreamDto, DLiveErrorType>.Success(dliveStreamsPage1));

            dliveApiStub.Setup(m => m.GetLiveStreams(1, 1, StreamSortOrder.Trending))
                .ReturnsAsync(MaybeResult<DLiveStreamDto, DLiveErrorType>.Success(dliveStreamsPage2));

            var dliveStreamProvider = new DLiveStreamProvider("", dliveApiStub.Object);

            var streamsPage1 = await dliveStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1, string.Empty);
            var streamsPage2 = await dliveStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1, streamsPage1.NextPageToken);

            Assert.AreEqual(streamsPage2.Items.Count(), 1);
            Assert.NotNull(streamsPage2.NextPageToken);
            Assert.AreEqual(streamsPage2.Items.First().StreamTitle, dliveStreamsPage2.data.livestreams.list.First().title);
        }

        [Test]
        public async Task Should_Not_Retrun_A_Page_Token_When_There_Are_No_More_Streams()
        {
            var dliveStreamsPage = new DLiveStreamDto
            {
                data = new DLiveStreamDataDto
                {
                    livestreams = new DLiveStreamsDto
                    {
                        list = new List<DLiveStreamItemDto>()
                    }
                }
            };

            dliveApiStub.Setup(m => m.GetLiveStreams(1, 0, StreamSortOrder.Trending))
                .ReturnsAsync(MaybeResult<DLiveStreamDto, DLiveErrorType>.Success(dliveStreamsPage));

            var dliveStreamProvider = new DLiveStreamProvider("", dliveApiStub.Object);

            var streams = await dliveStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1, string.Empty);

            Assert.IsFalse(streams.Items.Any());
            Assert.Null(streams.NextPageToken);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_Of_Streams_When_There_Is_An_Api_Error()
        {
            dliveApiStub.Setup(m => m.GetLiveStreams(1, 0, StreamSortOrder.Trending))
                .ReturnsAsync(MaybeResult<DLiveStreamDto, DLiveErrorType>.Fail(DLiveErrorType.ProviderNotAvailable));

            var dliveStreamProvider = new DLiveStreamProvider("", dliveApiStub.Object);

            var streams = await dliveStreamProvider.GetLiveStreams(new StreamFilterOptions(), 1, string.Empty);

            Assert.AreEqual(streams.Items.Count(), 0);
        }

        [Test]
        public async Task Should_Return_Streamer_Channel_If_A_Channel_Was_Found_And_The_Name_Matched()
        {
            dliveApiStub.Setup(m => m.GetUserByDisplayName("Test streamer")).ReturnsAsync(
                MaybeResult<DLiveUserDto, DLiveErrorType>.Success(new DLiveUserDto
                {
                    displayName = "Test Streamer"
                }
            ));

            var dliveStreamProvider = new DLiveStreamProvider("", dliveApiStub.Object);

            var streamerChannel = await dliveStreamProvider.GetStreamerChannel("Test streamer");

            Assert.IsNotNull(streamerChannel);
        }

        [Test]
        public async Task Should_Return_Nothing_If_A_Channel_Was_Not_Found()
        {
            dliveApiStub.Setup(m => m.GetUserByDisplayName("Test streamer"))
                .Returns(Task.FromResult(MaybeResult<DLiveUserDto, DLiveErrorType>.Success(Maybe<DLiveUserDto>.Nothing)));

            var dliveStreamProvider = new DLiveStreamProvider("", dliveApiStub.Object);

            var streamerChannel = await dliveStreamProvider.GetStreamerChannel("Test streamer");

            Assert.IsTrue(streamerChannel.Value.IsNothing);
        }

        [Test]
        public async Task Should_Return_Failure_If_The_Service_Is_Not_Available_When_Getting_Channels()
        {
            dliveApiStub.Setup(m => m.GetUserByDisplayName("Test streamer"))
                .Returns(Task.FromResult(MaybeResult<DLiveUserDto, DLiveErrorType>.Fail(DLiveErrorType.ProviderNotAvailable)));

            var dliveStreamProvider = new DLiveStreamProvider("", dliveApiStub.Object);

            var streamerChannel = await dliveStreamProvider.GetStreamerChannel("Test streamer");

            Assert.IsTrue(streamerChannel.IsFailure);
        }
    }
}
