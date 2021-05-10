using System.Collections.Generic;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.StreamProviders.Const;
using GameStreamSearch.StreamProviders.YouTube.Gateways.Dto.V3;
using GameStreamSearch.StreamProviders.YouTube.Mappers.V3;
using GameStreamSearch.Types;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.StreamProviders.YouTube.Mappers
{
    public class YouTubeChannelMapperTests
    {
        private string youtubeWebUrl = "http://youtube.com";

        [Test]
        public void Should_Include_A_Correctly_Structured_Channel_Url()
        {
            var channel = new YouTubeChannelDto
            {
                id = "testchannelid",
                snippet = new YouTubeChannelSnippetDto
                {
                    title = "testchannel",
                    thumbnails = new YouTubeChannelSnippetThumbnailsDto
                    {
                        @default = new YouTubeChannelSnippetThumbnailDto
                        {
                            url = "http://thumbnail.url"
                        }
                    }
                }
            };

            var platformChannel = new YouTubeChannelMapper(youtubeWebUrl).Map(channel);

            Assert.AreEqual(platformChannel.ChannelName, "testchannel");
            Assert.AreEqual(platformChannel.AvatarUrl, "http://thumbnail.url");
            Assert.AreEqual(platformChannel.StreamPlatformName, StreamPlatform.YouTube);
            Assert.AreEqual(platformChannel.ChannelUrl, "http://youtube.com/channel/testchannelid");

        }
    }
}
