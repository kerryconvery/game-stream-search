using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamProviders.Dto.DLive;

namespace GameStreamSearch.StreamProviders.Mappers
{
    public class DLiveMapper
    {
        private readonly string dliveWebUrl;

        public DLiveMapper(string dliveWebUrl)
        {
            this.dliveWebUrl = dliveWebUrl;
        }

        public Streams ToGameStreamsDto(IEnumerable<DLiveStreamItemDto> streams, NumericPageOffset nextPageOffset)
        {
            return new Streams(
                streams.Select(stream => new Stream
                {
                    StreamTitle = stream.title,
                    StreamerName = stream.creator.displayName,
                    StreamThumbnailUrl = stream.thumbnailUrl,
                    StreamerAvatarUrl = stream.creator.avatar,
                    StreamUrl = $"{dliveWebUrl}/{stream.creator.displayName}",
                    StreamPlatformName = StreamPlatformType.DLive.GetFriendlyName(),
                    IsLive = true,
                    Views = stream.watchingCount,
                }),
                nextPageOffset
            );
        }

        public PlatformChannel ToStreamerChannelDto(DLiveUserDto user)
        {
            return new PlatformChannel
            {
                ChannelName = user.displayName,
                AvatarUrl = user.avatar,
                ChannelUrl = $"{dliveWebUrl}/{user.displayName}",
                Platform = StreamPlatformType.DLive,
            };
        }
    }
}
