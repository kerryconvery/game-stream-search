﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Services.Dto;
using GameStreamSearch.StreamProviders.Builders;
using GameStreamSearch.StreamProviders.ProviderApi.DLive.Dto;
using GameStreamSearch.StreamProviders.ProviderApi.DLive.Interfaces;
using Moq;
using NUnit.Framework;

namespace GameStreamSearch.StreamProviders.Tests
{
    public class DLiveStreamProviderTests
    {
        [Test]
        public async Task Should_Return_Streams_And_A_Next_Page_Token_When_There_Are_No_Filters_Set()
        {
            var streamWatchUrl = "http://stream.watch.url";

            var dliveStreams = new DLiveStreamDto
            {
                data = new DLiveStreamDataDto
                {
                    livestreams = new DLiveLiveStreamsDto
                    {
                        list = new List<DLiveLiveStreamItemDto>
                        {
                            new DLiveLiveStreamItemDto
                            {
                                title = "fake dlive stream",
                                thumbnailUrl = "http://fake.dlive.thumbnail",
                                watchingCount = 100,
                                creator = new DLiveLiveStreamCreatorDto
                                {
                                    displayName = "FakeStreamer",
                                    avatar = "http://fake.avatar.url"
                                }
                            }
                        }
                    }
                }
            };

            var dliveApiStub = new Mock<IDLiveApi>();
            dliveApiStub.Setup(m => m.GetLiveStreams(1, 0, StreamSortOrder.Trending)).ReturnsAsync(dliveStreams);

            var dliveWatchUrlBuilderStub = new Mock<IDLiveWatchUrlBuilder>();
            dliveWatchUrlBuilderStub.Setup(m => m.Build("FakeStreamer")).Returns(streamWatchUrl);

            var dliveStreamProvider = new DLiveStreamProvider("DLive", dliveWatchUrlBuilderStub.Object, dliveApiStub.Object);

            var streams = await dliveStreamProvider.GetLiveStreams(new StreamFilterOptionsDto(), 1);

            Assert.AreEqual(streams.Items.Count(), 1);
            Assert.NotNull(streams.NextPageToken);
            Assert.AreEqual(streams.Items.First().StreamTitle, dliveStreams.data.livestreams.list.First().title);
            Assert.AreEqual(streams.Items.First().StreamUrl, streamWatchUrl);
            Assert.AreEqual(streams.Items.First().StreamerAvatarUrl, dliveStreams.data.livestreams.list.First().creator.avatar);
            Assert.AreEqual(streams.Items.First().StreamThumbnailUrl, dliveStreams.data.livestreams.list.First().thumbnailUrl);
            Assert.AreEqual(streams.Items.First().StreamerName, dliveStreams.data.livestreams.list.First().creator.displayName);
            Assert.AreEqual(streams.Items.First().Views, dliveStreams.data.livestreams.list.First().watchingCount);
            Assert.AreEqual(streams.Items.First().PlatformName, "DLive");
            Assert.AreEqual(streams.Items.First().IsLive, true);
        }


        [Test]
        public async Task Should_Return_No_Streams_When_A_Filter_Is_Applied()
        {
            var dliveApiStub = new Mock<IDLiveApi>();
            var dliveWatchUrlBuilderStub = new Mock<IDLiveWatchUrlBuilder>();

            var dliveStreamProvider = new DLiveStreamProvider("DLive", dliveWatchUrlBuilderStub.Object, dliveApiStub.Object);

            var streams = await dliveStreamProvider.GetLiveStreams(new StreamFilterOptionsDto { GameName = "some game" }, 1);

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
                    livestreams = new DLiveLiveStreamsDto
                    {
                        list = new List<DLiveLiveStreamItemDto>
                        {
                            new DLiveLiveStreamItemDto
                            {
                                title = "fake dlive stream page 1",
                                thumbnailUrl = "http://fake.dlive.thumbnail",
                                watchingCount = 100,
                                creator = new DLiveLiveStreamCreatorDto
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
                    livestreams = new DLiveLiveStreamsDto
                    {
                        list = new List<DLiveLiveStreamItemDto>
                        {
                            new DLiveLiveStreamItemDto
                            {
                                title = "fake dlive stream page 2",
                                thumbnailUrl = "http://fake.dlive.thumbnail",
                                watchingCount = 100,
                                creator = new DLiveLiveStreamCreatorDto
                                {
                                    displayName = "FakeStreamer",
                                    avatar = "http://fake.avatar.url"
                                }
                            }
                        }
                    }
                }
            };

            var dliveApiStub = new Mock<IDLiveApi>();
            dliveApiStub.Setup(m => m.GetLiveStreams(1, 0, StreamSortOrder.Trending)).ReturnsAsync(dliveStreamsPage1);
            dliveApiStub.Setup(m => m.GetLiveStreams(1, 1, StreamSortOrder.Trending)).ReturnsAsync(dliveStreamsPage2);

            var dliveWatchUrlBuilderStub = new Mock<IDLiveWatchUrlBuilder>();

            var dliveStreamProvider = new DLiveStreamProvider("DLive", dliveWatchUrlBuilderStub.Object, dliveApiStub.Object);

            var streamsPage1 = await dliveStreamProvider.GetLiveStreams(new StreamFilterOptionsDto(), 1);
            var streamsPage2 = await dliveStreamProvider.GetLiveStreams(new StreamFilterOptionsDto(), 1, streamsPage1.NextPageToken);


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
                    livestreams = new DLiveLiveStreamsDto
                    {
                        list = new List<DLiveLiveStreamItemDto>()
                    }
                }
            };

            var dliveApiStub = new Mock<IDLiveApi>();
            dliveApiStub.Setup(m => m.GetLiveStreams(1, 0, StreamSortOrder.Trending)).ReturnsAsync(dliveStreamsPage);

            var dliveWatchUrlBuilderStub = new Mock<IDLiveWatchUrlBuilder>();

            var dliveStreamProvider = new DLiveStreamProvider("DLive", dliveWatchUrlBuilderStub.Object, dliveApiStub.Object);

            var streams = await dliveStreamProvider.GetLiveStreams(new StreamFilterOptionsDto(), 1);

            Assert.IsFalse(streams.Items.Any());
            Assert.Null(streams.NextPageToken);
        }
    }
}