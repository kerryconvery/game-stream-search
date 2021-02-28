using GameStreamSearch.Types;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.StreamProviders.DLive.Gateways.Dto;
using GameStreamSearch.StreamProviders.Const;

namespace GameStreamSearch.StreamProviders.DLive.Mappers
{
    public class DLiveChannelMapper
    {
        private readonly string dliveWebUrl;

        public DLiveChannelMapper(string dliveWebUrl)
        {
            this.dliveWebUrl = dliveWebUrl;
        }

        public MaybeResult<PlatformChannelDto, StreamProviderError> Map(
            MaybeResult<DLiveUserDto, StreamProviderError> userSearchResult)
        {
            return userSearchResult.Select(user =>
            {
                return new PlatformChannelDto
                {
                    ChannelName = user.displayName,
                    AvatarUrl = user.avatar,
                    ChannelUrl = $"{dliveWebUrl}/{user.displayName}",
                    StreamPlatformName = StreamPlatform.DLive,
                };
            });
        }
    }
}
