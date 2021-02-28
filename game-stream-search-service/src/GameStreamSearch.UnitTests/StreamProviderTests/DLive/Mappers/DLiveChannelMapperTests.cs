using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.StreamProviders.Const;
using GameStreamSearch.StreamProviders.DLive.Gateways.Dto;
using GameStreamSearch.StreamProviders.DLive.Mappers;
using GameStreamSearch.Types;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.StreamProviders.DLive.Mappers
{
    public class DLiveChannelMapperTests
    {
        private string dliveUrl = "http://dlive.com";

        [Test]
        public void Should_Map_A_DLive_Channel_To_A_PlatformChannel()
        {
            var mapper = new DLiveChannelMapper(dliveUrl);
            var dliveUser = new DLiveUserDto
            {
                displayName = "testuser",
                avatar = "http://avatar.url"
            };
            var userSearchResults = MaybeResult<DLiveUserDto, StreamProviderError>.Success(dliveUser);

            var platformChannl = mapper.Map(userSearchResults).GetOrElse(new PlatformChannelDto());

            Assert.AreEqual(platformChannl.ChannelName, "testuser");
            Assert.AreEqual(platformChannl.AvatarUrl, "http://avatar.url");
            Assert.AreEqual(platformChannl.StreamPlatformName, StreamPlatform.DLive);
            Assert.AreEqual(platformChannl.ChannelUrl, "http://dlive.com/testuser");
        }
    }
}
