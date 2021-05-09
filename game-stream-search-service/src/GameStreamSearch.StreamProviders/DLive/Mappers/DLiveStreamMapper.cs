using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Types;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.StreamProviders.DLive.Gateways.Dto;
using GameStreamSearch.StreamProviders.Const;

namespace GameStreamSearch.StreamProviders.DLive.Mappers
{
    public class DLiveStreamMapper
    {
        private readonly string dliveWebUrl;

        public DLiveStreamMapper(string dliveWebUrl)
        {
            this.dliveWebUrl = dliveWebUrl;
        }

        public PlatformStreamsDto Map(
            IEnumerable<DLiveStreamItemDto> streams,
            int pageSize,
            int pageOffset
        )
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
        }
    }
}
