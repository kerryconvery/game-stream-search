using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Application.Types;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.DomainServiceTests
{
    public class StreamAggregationServiceTests
    {
        [Test]
        public void Should_Pack_A_Collection_Of_Next_Page_Token_Down_Into_A_Single_Page_Token_When_Aggregating_Streams()
        {
            var streamCollection = new List<PlatformStreamsDto>
            {
                new PlatformStreamsDto { Streams = new List<PlatformStreamDto> { new PlatformStreamDto() }, NextPageToken =  "page token 1" },
                new PlatformStreamsDto { Streams = new List<PlatformStreamDto> { new PlatformStreamDto() }, NextPageToken = "page token 2" },
            };
            var streamAggregationService = new PageTokenService();

            var packedToken = streamAggregationService.PackPageTokens(streamCollection.ToDictionary(s => s.StreamPlatformName, s => s.NextPageToken));

            var decodedTokens = streamAggregationService.UnpackPageToken(packedToken);

            Assert.AreEqual(decodedTokens.Keys.Count(), 2);
            Assert.AreEqual(decodedTokens.Values.First(), "page token 1");
            Assert.AreEqual(decodedTokens.Values.Last(), "page token 2");
        }
    }
}
