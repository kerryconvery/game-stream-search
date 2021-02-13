using System.Linq;
using System.Collections.Generic;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.StreamProviders.Dto.DLive;
using GameStreamSearch.StreamProviders.Mappers;
using GameStreamSearch.Types;
using NUnit.Framework;
using GameStreamSearch.UnitTests.Extensions;

namespace GameStreamSearch.UnitTests.StreamProviders.Mappers
{
    public class DLiveeStreamMapperTests
    {
        private string dliveUrl = "dlive.url";
        private NumericPageOffset pageOffset = new NumericPageOffset(2, "0");
        private MaybeResult<IEnumerable<DLiveStreamItemDto>, StreamProviderError> streamSearchResults;
        private DLiveStreamMapper mapper;

        [SetUp]
        public void Setup()
        {
            var dliveStreams = new List<DLiveStreamItemDto>
            {
                new DLiveStreamItemDto { creator = new DLiveUserDto { displayName = "TestUserA" } },
                new DLiveStreamItemDto { creator = new DLiveUserDto { displayName = "TestUserB" } },
            };

            streamSearchResults = MaybeResult<IEnumerable<DLiveStreamItemDto>, StreamProviderError>.Success(dliveStreams);
            mapper = new DLiveStreamMapper(dliveUrl);
        }

        [Test]
        public void Should_Map_A_DLive_Channel_To_A_PlatformChannel()
        {
            var streams = mapper.Map(streamSearchResults, pageOffset);

            Assert.AreEqual(streams.Items.Select(s => s.StreamUrl).First(), "dlive.url/TestUserA");
            Assert.AreEqual(streams.Items.Select(s => s.StreamPlatformName).First(), StreamPlatformType.DLive.GetFriendlyName());
        }

        [Test]
        public void Should_Have_A_Next_Page_Token_When_The_Number_Of_Streams_Found_Is_Equal_To_The_Page_Size()
        {
            var streams = mapper.Map(streamSearchResults, pageOffset);

            Assert.AreEqual(streams.NextPageToken, "2");
        }

        [Test]
        public void Should_Not_Have_A_Next_Page_Token_When_The_Number_Of_Streams_Returned_Is_Less_Than_The_Page_Size()
        {
            var streams = mapper.Map(streamSearchResults, new NumericPageOffset(3, "0"));

            Assert.IsEmpty(streams.NextPageToken);
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
