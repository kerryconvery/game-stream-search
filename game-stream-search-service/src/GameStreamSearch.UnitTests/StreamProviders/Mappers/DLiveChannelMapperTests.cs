using System;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.StreamProviders.Dto.DLive;
using GameStreamSearch.StreamProviders.Mappers;
using GameStreamSearch.Types;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.StreamProviders.Mappers
{
    public class DLiveChannelMapperTests
    {
        private string dliveUrl = "dlive.url";

        [Test]
        public void Should_Map_A_DLive_Channel_To_A_PlatformChannel()
        {
            var mapper = new DLiveChannelMapper(dliveUrl);
            var dliveUser = new DLiveUserDto { displayName = "testuser" };
            var userSearchResults = MaybeResult<DLiveUserDto, StreamProviderError>.Success(dliveUser);

            var platformChannl = mapper.Map(userSearchResults).GetOrElse(new PlatformChannel());

            Assert.AreEqual(platformChannl.ChannelUrl, "dlive.url/testuser");
            Assert.AreEqual(platformChannl.Platform, StreamPlatformType.DLive);
        }
    }
}
