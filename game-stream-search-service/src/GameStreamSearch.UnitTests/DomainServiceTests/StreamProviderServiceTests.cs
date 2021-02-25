using System;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Application.Models;
using GameStreamSearch.Types;
using NUnit.Framework;
using GameStreamSearch.Domain.Entities;
using GameStreamSearch.Application.Dto;

namespace GameStreamSearch.UnitTests.DomainServiceTests
{
    public class StreamProviderServiceTests
    {
        private StreamPlatform twitchPlatform = new StreamPlatform("twitch");
        private StreamPlatform youTubePlatform = new StreamPlatform("youtube");

        [Test]
        public void Should_Return_A_List_Of_Providers_That_Support_The_Selected_Filters()
        {
            var streamProviderService = new StreamProviderService()
                .RegisterStreamProvider(new FakeProvider(youTubePlatform, true))
                .RegisterStreamProvider(new FakeProvider(twitchPlatform, false));

            var streamSources = streamProviderService.GetSupportingPlatforms(new StreamFilterOptions());

            Assert.AreEqual(streamSources.Count(), 1);
            Assert.AreEqual(streamSources.First(), youTubePlatform.Name);
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

        Task<PlatformStreamsDto> IStreamProvider.GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, PageToken pageToken)
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
