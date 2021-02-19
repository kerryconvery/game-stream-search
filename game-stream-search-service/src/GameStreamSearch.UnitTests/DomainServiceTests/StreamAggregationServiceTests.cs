using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Application.Dto;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.DomainServiceTests
{
    public class StreamAggregationServiceTests
    {
        [Test]
        public void Should_Reduce_A_Collection_Of_Streams_Down_To_A_Single_Unified_Set_Of_Streams()
        {
            var StreamA = new PlatformStreamDto();
            var StreamB = new PlatformStreamDto();
            var streamCollection = new List<PlatformStreamsDto>
            {
                new PlatformStreamsDto { Streams = new List<PlatformStreamDto> { StreamA }, NextPageToken = "" },
                new PlatformStreamsDto { Streams = new List<PlatformStreamDto> { StreamB }, NextPageToken = "" },
            };
            var streamAggregationService = new StreamAggregationService();

            var aggregatedStreams = streamAggregationService.AggregateStreams(streamCollection);

            Assert.AreEqual(aggregatedStreams.Streams.Count(), 2);
            Assert.True(aggregatedStreams.Streams.Contains(StreamA));
            Assert.True(aggregatedStreams.Streams.Contains(StreamB));
        }

        [Test]
        public void Should_Pack_A_Collection_Of_Next_Page_Token_Down_Into_A_Single_Page_Token_When_Aggregating_Streams()
        {
            var streamCollection = new List<PlatformStreamsDto>
            {
                new PlatformStreamsDto { Streams = new List<PlatformStreamDto> { new PlatformStreamDto() }, NextPageToken =  "page token 1" },
                new PlatformStreamsDto { Streams = new List<PlatformStreamDto> { new PlatformStreamDto() }, NextPageToken = "page token 2" },
            };
            var streamAggregationService = new StreamAggregationService();

            var aggregatedStreams = streamAggregationService.AggregateStreams(streamCollection);

            var decodedTokens = streamAggregationService.UnpackPageToken(aggregatedStreams.NextPageToken);

            Assert.AreEqual(decodedTokens.Keys.Count(), 2);
            Assert.AreEqual(decodedTokens.Values.First(), "page token 1");
            Assert.AreEqual(decodedTokens.Values.Last(), "page token 2");
        }
    }
}
