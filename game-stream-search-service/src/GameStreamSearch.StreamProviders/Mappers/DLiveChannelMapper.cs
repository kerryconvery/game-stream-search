using System;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.StreamProviders.Dto.DLive;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.Mappers
{
    public class DLiveChannelMapper
    {
        private readonly string dliveWebUrl;

        public DLiveChannelMapper(string dliveWebUrl)
        {
            this.dliveWebUrl = dliveWebUrl;
        }

        public MaybeResult<PlatformChannel, StreamProviderError> Map(MaybeResult<DLiveUserDto, StreamProviderError> userSearchResult)
        {
            return userSearchResult.Select(user =>
            {
                return new PlatformChannel
                {
                    ChannelName = user.displayName,
                    AvatarUrl = user.avatar,
                    ChannelUrl = $"{dliveWebUrl}/{user.displayName}",
                    Platform = StreamPlatformType.DLive,
                };
            });
        }
    }
}
