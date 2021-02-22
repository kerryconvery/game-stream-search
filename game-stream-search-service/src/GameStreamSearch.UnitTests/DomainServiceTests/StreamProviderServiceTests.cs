﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Application.Types;
using GameStreamSearch.Types;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.DomainServiceTests
{
    public class StreamProviderServiceTests
    {
        private StreamPlatform twitchPlatform = new StreamPlatform("twitch");
        private StreamPlatform youTubePlatform = new StreamPlatform("youtube");


        [Test]
        public void Should_Return_A_List_Of_Stream_Sources_With_Page_Tokens_That_Support_The__Selected_Filters()
        {
            var streamProviderService = new StreamProviderService()
                .RegisterStreamProvider(new FakeProvider(youTubePlatform, true))
                .RegisterStreamProvider(new FakeProvider(twitchPlatform, false));

            var pageTokens = new Dictionary<string, string>
            {
                { "youtube", "youtube token" },
                { "twitch", "twitch token" },
            };

            var streamSources = streamProviderService.CreateStreamSources(pageTokens, new StreamFilterOptions());

            Assert.AreEqual(streamSources.Count(), 1);
            Assert.AreEqual(streamSources.First().StreamPlatformName, youTubePlatform.Name);
            Assert.AreEqual(streamSources.First().PageToken, "youtube token");
        }

        [Test]
        public void Should_Default_The_Platform_Page_Token_To_An_Empty_String_When_It_Is_Missing_From_The_List_Of_Page_Tokens()
        {
            var streamProviderService = new StreamProviderService()
                .RegisterStreamProvider(new FakeProvider(twitchPlatform, true));

            var pageTokens = new Dictionary<string, string>();

            var streamSources = streamProviderService.CreateStreamSources(pageTokens, new StreamFilterOptions());

            Assert.AreEqual(streamSources.Count(), 1);
            Assert.AreEqual(streamSources.First().PageToken, string.Empty);
        }
    }

    internal class FakeProvider : IStreamProvider
    {
        private readonly StreamPlatform streamPlatform;
        private readonly bool isFilterSupported;

        public FakeProvider(StreamPlatform streamPlatform, bool isFilterSupported)
        {
            this.streamPlatform = streamPlatform;
            this.isFilterSupported = isFilterSupported;
        }

        Task<PlatformStreamsDto> IStreamProvider.GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken)
        {
            throw new NotImplementedException();
        }

        Task<MaybeResult<PlatformChannelDto, StreamProviderError>> IStreamProvider.GetStreamerChannel(string channelName)
        {
            throw new NotImplementedException();
        }

        public StreamPlatform StreamPlatform => streamPlatform;

        public bool AreFilterOptionsSupported(StreamFilterOptions filterOptions) => isFilterSupported;
    }
}
