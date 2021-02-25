using GameStreamSearch.Application.Mappers;
using GameStreamSearch.Types;
using NUnit.Framework;
using GameStreamSearch.Gateways.Dto.DLive;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Domain.Entities;

namespace GameStreamSearch.UnitTests.StreamProviders.Mappers
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
            Assert.AreEqual(platformChannl.StreamPlatformName, StreamPlatform.DLive.Name);
            Assert.AreEqual(platformChannl.ChannelUrl, "http://dlive.com/testuser");
        }
    }
}
