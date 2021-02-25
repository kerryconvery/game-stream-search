using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.Models;
using GameStreamSearch.Types;
using GameStreamSearch.Gateways.Dto.DLive;
using GameStreamSearch.Domain.Entities;
using GameStreamSearch.Application.Dto;

namespace GameStreamSearch.StreamProviders.Mappers
{
    public class DLiveStreamMapper
    {
        private readonly string dliveWebUrl;

        public DLiveStreamMapper(string dliveWebUrl)
        {
            this.dliveWebUrl = dliveWebUrl;
        }

        public PlatformStreamsDto Map(
            MaybeResult<IEnumerable<DLiveStreamItemDto>, StreamProviderError> streamSearchResults,
            int pageSize,
            int pageOffset
        )
        {
            return streamSearchResults.Select(streams =>
            {
                return new PlatformStreamsDto
                {
                    StreamPlatformName = StreamPlatform.DLive,
                    Streams = streams.Select(stream =>
                    {
                        return new PlatformStreamDto
                        {
                            StreamTitle = stream.title,
                            StreamerName = stream.creator.displayName,
                            StreamThumbnailUrl = stream.thumbnailUrl,
                            StreamerAvatarUrl = stream.creator.avatar,
                            StreamUrl = $"{dliveWebUrl}/{stream.creator.displayName}",
                            IsLive = true,
                            Views = stream.watchingCount,
                        };
                    }),
                    NextPageToken = streams.Count() == pageSize ? (pageOffset + pageSize).ToString() : string.Empty
                };
            }).GetOrElse(PlatformStreamsDto.Empty(StreamPlatform.DLive));
        }

        internal PlatformStreamsDto Map(MaybeResult<IEnumerable<DLiveStreamItemDto>, StreamProviderError> liveStreamsResult, int pageSize, PageToken pageToken)
        {
            throw new NotImplementedException();
        }
    }
}
