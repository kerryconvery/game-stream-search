using System;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application.Services.StreamProvider;
using GameStreamSearch.Application.StreamProvider;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.Types;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.DomainServiceTests
{
    public class StreamProviderServiceTests
    {
        private string twitchPlatform = "twitch";
        private string youTubePlatform = "youtube";

        [Test]
        public void Should_Return_A_List_Of_Providers_That_Support_The_Selected_Filters()
        {
            var streamPlatformService = new StreamPlatformService()
                .RegisterStreamProvider(new FakeProvider(youTubePlatform, true))
                .RegisterStreamProvider(new FakeProvider(twitchPlatform, false));

            var streamSources = streamPlatformService.GetSupportingPlatforms(new StreamFilterOptions());

            Assert.AreEqual(streamSources.Count(), 1);
            Assert.AreEqual(streamSources.First(), youTubePlatform);
        }
    }

    internal class FakeProvider : IStreamProvider
    {
        private readonly string streamPlatformName;
        private readonly bool isFilterSupported;

        public FakeProvider(string streamPlatformName, bool isFilterSupported)
        {
            this.streamPlatformName = streamPlatformName;
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

        public string StreamPlatformName => streamPlatformName;

        public bool AreFilterOptionsSupported(StreamFilterOptions filterOptions) => isFilterSupported;
    }
}
