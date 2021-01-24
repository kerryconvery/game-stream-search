using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Base64Url;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamPlatformApi;
using GameStreamSearch.Application;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders
{
    public class DLiveStreamProvider : IStreamProvider
    {
        private readonly string baseUrl;
        private readonly IDLiveApi dliveApi;

        public DLiveStreamProvider(string baseUrl, IDLiveApi dliveApi)
        {
            this.baseUrl = baseUrl;
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

        public async Task<GameStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken = null)
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
                    Items = result.data.livestreams.list.Select(s => new GameStreamDto
                    {
                        StreamTitle = s.title,
                        StreamerName = s.creator.displayName,
                        StreamThumbnailUrl = s.thumbnailUrl,
                        StreamerAvatarUrl = s.creator.avatar,
                        StreamUrl = $"{baseUrl}/{s.creator.displayName}",
                        StreamPlatformName = Platform.GetFriendlyName(),
                        IsLive = true,
                        Views = s.watchingCount,
                    }),
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

            return MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Success(
                userResult.Value.Map(c => new StreamerChannelDto
                {
                    ChannelName = c.displayName,
                    AvatarUrl = c.avatar,
                    ChannelUrl = $"{baseUrl}/{c.displayName}",
                    Platform = Platform,
                })
            );
        }

        public StreamPlatformType Platform => StreamPlatformType.DLive;
    }
}
