using System.Linq;
using System.Collections.Generic;
using GameStreamSearch.Application;
using GameStreamSearch.StreamProviders.Dto.DLive;
using GameStreamSearch.StreamProviders.Mappers;
using GameStreamSearch.Types;
using NUnit.Framework;
using GameStreamSearch.UnitTests.Extensions;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.UnitTests.StreamProviders.Mappers
{
    public class DLiveeStreamMapperTests
    {
        private string dliveUrl = "dlive.url";
        private int pageSize = 1;
        private int pageOffset = 0;
        private MaybeResult<IEnumerable<DLiveStreamItemDto>, StreamProviderError> streamSearchResults;
        private DLiveStreamMapper dliveStreamMapper;

        [SetUp]
        public void Setup()
        {
            var dliveStreams = new List<DLiveStreamItemDto>
            {
                new DLiveStreamItemDto
                {
                    title = "test stream",
                    thumbnailUrl = "http://thunmbnail.url",
                    watchingCount = 1,
                    creator = new DLiveUserDto { displayName = "TestUserA", avatar = "http://avatar.url" }
                },
            };

            streamSearchResults = MaybeResult<IEnumerable<DLiveStreamItemDto>, StreamProviderError>.Success(dliveStreams);
            dliveStreamMapper = new DLiveStreamMapper(dliveUrl);
        }

        [Test]
        public void Should_Map_DLive_Streams_To_Streams()
        {
            var streams = dliveStreamMapper.Map(streamSearchResults, pageSize, pageOffset);

            Assert.AreEqual(streams.Streams.First().StreamTitle, "test stream");
            Assert.AreEqual(streams.Streams.First().StreamerName, "TestUserA");
            Assert.AreEqual(streams.Streams.First().StreamThumbnailUrl, "http://thunmbnail.url");
            Assert.AreEqual(streams.Streams.First().StreamerAvatarUrl, "http://avatar.url");
            Assert.AreEqual(streams.Streams.First().StreamUrl, "dlive.url/TestUserA");
            Assert.AreEqual(streams.Streams.First().Views, 1);
            Assert.AreEqual(streams.Streams.First().IsLive, true);
            Assert.AreEqual(streams.StreamPlatform, StreamPlatformType.DLive);
        }

        [Test]
        public void Should_Return_The_Next_Page_Token_When_The_Number_Of_Streams_Is_Equal_To_The_Page_Size()
        {
            var platformStreams = dliveStreamMapper.Map(streamSearchResults, 1, 0);

            Assert.AreEqual(platformStreams.NextPageToken, "1");
        }

        [Test]
        public void Should_Return_An_Empty_Next_Page_Token_When_The_Number_Of_Streams_Is_Less_Than_The_Page_Size()
        {
            var emptySearchResults = MaybeResult<IEnumerable<DLiveStreamItemDto>, StreamProviderError>
                .Success(new List<DLiveStreamItemDto>());

            var streams = dliveStreamMapper.Map(emptySearchResults, pageSize, pageOffset);

            Assert.IsTrue(streams.IsEmpty());
            Assert.IsEmpty(streams.NextPageToken);
        }

        [Test]
        public void Should_Return_An_Empty_List_Of_Streams_When_No_Streams_Where_Returned_From_The_Streaming_Platform()
        {
            var emptySearchResults = MaybeResult<IEnumerable<DLiveStreamItemDto>, StreamProviderError>
                .Success(new List<DLiveStreamItemDto>());

            var streams = dliveStreamMapper.Map(emptySearchResults, pageSize, pageOffset);

            Assert.IsTrue(streams.IsEmpty());
            Assert.IsEmpty(streams.NextPageToken);
        }
    }
}
