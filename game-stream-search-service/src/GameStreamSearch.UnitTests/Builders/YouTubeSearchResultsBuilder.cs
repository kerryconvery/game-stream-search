using System;
using System.Collections.Generic;
using GameStreamSearch.Application.Dto.YouTube.YouTubeV3;

namespace GameStreamSearch.UnitTests.Builders
{
    internal class YouTubeSearchResultsBuilder
    {
        private List<YouTubeSearchItemDto> youTubeSearchItemDtos = new List<YouTubeSearchItemDto>();
        private string nextPageToken = string.Empty;

        public YouTubeSearchResultsBuilder Add(string videoId, string streamTitle, string channelTitle, string channelId, string streamThumbnailUrl)
        {
            var videoSearchItemDto = new YouTubeSearchItemDto
            {
                id = new YouTubeSearchItemIdDto
                {
                    videoId = videoId,
                },

                snippet = new YouTubeSearchSnippetDto
                {
                    channelId = channelId,
                    title = streamTitle,
                    channelTitle = channelTitle,
                    thumbnails = new YouTubeSearchSnippetThumbnailsDto
                    {
                        medium = new YouTubeSearchSnippetThumbnailDto { url = streamThumbnailUrl }
                    }
                }
            };
            youTubeSearchItemDtos.Add(videoSearchItemDto);

            return this;
        }

        public YouTubeSearchResultsBuilder SetNextPageToken(string nextPageToken)
        {
            this.nextPageToken = nextPageToken;
            return this;
        }

        public YouTubeSearchDto Build()
        {
            var youTubeSearchDto = new YouTubeSearchDto
            {
                items = youTubeSearchItemDtos,
                nextPageToken = nextPageToken
            };

            return youTubeSearchDto;
        }
    }
}
