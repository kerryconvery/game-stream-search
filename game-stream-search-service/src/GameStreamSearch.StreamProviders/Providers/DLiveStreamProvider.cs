using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Base64Url;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application;
using GameStreamSearch.Types;
using GameStreamSearch.StreamProviders.Dto.DLive;
using System.Collections.Generic;

namespace GameStreamSearch.StreamProviders
{
    public class DLiveStreamProvider : IStreamProvider
    {
        private readonly string dliveWebUrl;
        private readonly IDLiveApi dliveApi;

        public DLiveStreamProvider(string dliveWebUrl, IDLiveApi dliveApi)
        {
            this.dliveWebUrl = dliveWebUrl;
            this.dliveApi = dliveApi;
        }

        private int GetPageOffset(string nextPageToken)
        {
            if (string.IsNullOrEmpty(nextPageToken))
            {
                return 0;
            }

            var base64Decrypter = new Base64Decryptor(nextPageToken, new FromBase64Transform());

            return base64Decrypter.ReadInt32();
        }

        private string GetNextPageToken(bool hasStreams, int pageSize, int pageOffset)
        {
            if (!hasStreams)
            {
                return null;
            }

            var base64Encryptor = new Base64Encryptor(new ToBase64Transform());

            base64Encryptor.Write(pageOffset + pageSize);

            return base64Encryptor.ToString();
        }

        private GameStreamDto MapToGameStream(DLiveStreamItemDto streamItem)
        {
            return new GameStreamDto
            {
                StreamTitle = streamItem.title,
                StreamerName = streamItem.creator.displayName,
                StreamThumbnailUrl = streamItem.thumbnailUrl,
                StreamerAvatarUrl = streamItem.creator.avatar,
                StreamUrl = $"{dliveWebUrl}/{streamItem.creator.displayName}",
                StreamPlatformName = Platform.GetFriendlyName(),
                IsLive = true,
                Views = streamItem.watchingCount,
            };
        }

        public async Task<GameStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken)
        {
            //DLive does not support filtering streams
            if (!string.IsNullOrEmpty(filterOptions.GameName))
            {
                return GameStreamsDto.Empty;
            }

            var pageOffset = GetPageOffset(pageToken);

            var liveStreamsResult = await dliveApi.GetLiveStreams(pageSize, pageOffset, StreamSortOrder.Trending);

            if (liveStreamsResult.IsFailure)
            {
                return GameStreamsDto.Empty;
            }

            return liveStreamsResult.Value.Map(result =>
                new GameStreamsDto
                {
                    Items = result.data.livestreams.list.Select(MapToGameStream),
                    NextPageToken = GetNextPageToken(result.data.livestreams.list.Any(), pageSize, pageOffset),
                }
            ).GetOrElse(GameStreamsDto.Empty);
        }

        public async Task<MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>> GetStreamerChannel(string channelName)
        {
            var userResult = await dliveApi.GetUserByDisplayName(channelName);

            if (userResult.IsFailure)
            {
                return MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Fail(GetStreamerChannelErrorType.ProviderNotAvailable);
            }

            var streamerChannel = userResult.Value.Map(channel => channel.ToStreamerChannelDto(dliveWebUrl));

            return MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Success(streamerChannel);
        }

        public StreamPlatformType Platform => StreamPlatformType.DLive;
    }
}
