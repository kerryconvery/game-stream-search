using System;
using System.Collections.Generic;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.StreamProviders.Dto.YouTube.YouTubeV3;
using GameStreamSearch.StreamProviders.Mappers;
using GameStreamSearch.Types;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.StreamProviders.Mappers
{
    public class YouTubeChannelMapperTests
    {
        private string streamPlatformId = "youtube";
        private string youtubeWebUrl = "http://youtube.com";

        [Test]
        public void Should_Includ_A_Currectly_Structured_Channel_Url()
        {
            var channels = new List<YouTubeChannelDto>
            {
                new YouTubeChannelDto {
                    snippet = new YouTubeChannelSnippetDto {
                        title = "testchannel",
                        thumbnails = new YouTubeChannelSnippetThumbnailsDto
                        {
                            @default = new YouTubeChannelSnippetThumbnailDto
                            {
                                url = "http://thumbnail.url"
                            }
                        }
                    }
                }
            };
            var channelResults = MaybeResult<IEnumerable<YouTubeChannelDto>, StreamProviderError>.Success(channels);

            var channel = new YouTubeChannelMapper(youtubeWebUrl)
                .Map(streamPlatformId, channelResults)
                .GetOrElse(new PlatformChannelDto());

            Assert.AreEqual(channel.ChannelName, "testchannel");
            Assert.AreEqual(channel.AvatarUrl, "http://thumbnail.url");
            Assert.AreEqual(channel.StreamPlatformId, streamPlatformId);
            Assert.AreEqual(channel.ChannelUrl, "http://youtube.com/user/testchannel");

        }
    }
}
