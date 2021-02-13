using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamProviders.Dto.DLive;
using GameStreamSearch.Types;
using GameStreamSearch.Application;

namespace GameStreamSearch.StreamProviders.Mappers
{
    public class DLiveStreamMapper
    {
        private readonly string dliveWebUrl;

        public DLiveStreamMapper(string dliveWebUrl)
        {
            this.dliveWebUrl = dliveWebUrl;
        }

        public Streams Map(
            MaybeResult<IEnumerable<DLiveStreamItemDto>, StreamProviderError> streamSearchResults,
            NumericPageOffset pageOffset)
        {
            return streamSearchResults.Select(streams =>
            {
                return new Streams(
                    streams.Select(stream =>
                    {
                        return new Stream
                        {
                            StreamTitle = stream.title,
                            StreamerName = stream.creator.displayName,
                            StreamThumbnailUrl = stream.thumbnailUrl,
                            StreamerAvatarUrl = stream.creator.avatar,
                            StreamUrl = $"{dliveWebUrl}/{stream.creator.displayName}",
                            StreamPlatformName = StreamPlatformType.DLive.GetFriendlyName(),
                            IsLive = true,
                            Views = stream.watchingCount,
                        };
                    }),
                    pageOffset.GetNextOffset(streams.Count())
                );
            }).GetOrElse(Streams.Empty);
        }
    }
}
