using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Application.ValueObjects;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.DomainServiceTests
{
    public class StreamAggregationServiceTests
    {
        [Test]
        public void Should_Pack_A_Collection_Of_Next_Page_Token_Down_Into_A_Single_Page_Token_When_Aggregating_Streams()
        {
            var streamCollection = new List<PlatformStreams>
            {
                new PlatformStreams { Streams = new List<PlatformStream> { new PlatformStream() }, NextPageToken =  "page token 1" },
                new PlatformStreams { Streams = new List<PlatformStream> { new PlatformStream() }, NextPageToken = "page token 2" },
            };
            var streamAggregationService = new PageTokenService();

            var packedToken = streamAggregationService.PackPageTokens(streamCollection.ToDictionary(s => s.StreamPlatform.PlatformId, s => s.NextPageToken));

            var decodedTokens = streamAggregationService.UnpackPageToken(packedToken);

            Assert.AreEqual(decodedTokens.Keys.Count(), 2);
            Assert.AreEqual(decodedTokens.Values.First(), "page token 1");
            Assert.AreEqual(decodedTokens.Values.Last(), "page token 2");
        }
    }
}
