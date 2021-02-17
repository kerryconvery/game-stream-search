using System.Linq;
using System.Collections.Generic;
using GameStreamSearch.Application;
using GameStreamSearch.Application.ValueObjects;
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
        private NumericPageOffset pageOffset = new NumericPageOffset(1, "0");
        private MaybeResult<IEnumerable<DLiveStreamItemDto>, StreamProviderError> streamSearchResults;
        private DLiveStreamMapper mapper;

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
            mapper = new DLiveStreamMapper(dliveUrl);
        }

        [Test]
        public void Should_Map_DLive_Streams_To_Streams()
        {
            var streams = mapper.Map(streamSearchResults, pageOffset);

            Assert.AreEqual(streams.Items.First().StreamTitle, "test stream");
            Assert.AreEqual(streams.Items.First().StreamerName, "TestUserA");
            Assert.AreEqual(streams.Items.First().StreamThumbnailUrl, "http://thunmbnail.url");
            Assert.AreEqual(streams.Items.First().StreamerAvatarUrl, "http://avatar.url");
            Assert.AreEqual(streams.Items.First().StreamUrl, "dlive.url/TestUserA");
            Assert.AreEqual(streams.Items.First().Views, 1);
            Assert.AreEqual(streams.Items.First().IsLive, true);
            Assert.AreEqual(streams.Items.First().StreamPlatformName, StreamPlatformType.DLive.GetFriendlyName());
            Assert.AreEqual(streams.NextPageToken, "1");
        }

        [Test]
        public void Should_Return_An_Empty_List_Of_Streams_When_No_Streams_Where_Returned_From_The_Streaming_Platform()
        {
            var emptySearchResults = MaybeResult<IEnumerable<DLiveStreamItemDto>, StreamProviderError>
                .Success(new List<DLiveStreamItemDto>());

            var streams = mapper.Map(emptySearchResults, pageOffset);

            Assert.IsTrue(streams.IsEmpty());
            Assert.IsEmpty(streams.NextPageToken);
        }
    }
}
