using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Base64Url;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application;
using GameStreamSearch.StreamProviders.Builders;
using GameStreamSearch.Application.Exceptions;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamPlatformApi;

namespace GameStreamSearch.StreamProviders
{
    public class DLiveStreamProvider : IStreamProvider
    {
        private readonly IDLiveWatchUrlBuilder urlBuilder;
        private readonly IDLiveApi dliveApi;

        public DLiveStreamProvider(IDLiveWatchUrlBuilder urlBuilder, IDLiveApi dliveApi)
        {
            this.urlBuilder = urlBuilder;
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

        public async Task<GameStreamsDto> GetLiveStreams(StreamFilterOptionsDto filterOptions, int pageSize, string pageToken = null)
        {
            //DLive does not support filtering streams
            if (!string.IsNullOrEmpty(filterOptions.GameName))
            {
                return GameStreamsDto.Empty();
            }

            var pageOffset = GetPageOffset(pageToken);

            var response = await dliveApi.GetLiveStreams(pageSize, pageOffset, StreamSortOrder.Trending);

            return new GameStreamsDto
            {
                Items = response.data.livestreams.list.Select(s => new GameStreamDto
                {
                    StreamTitle = s.title,
                    StreamerName = s.creator.displayName,
                    StreamThumbnailUrl = s.thumbnailUrl,
                    StreamerAvatarUrl = s.creator.avatar,
                    StreamUrl = urlBuilder.Build(s.creator.displayName),
                    StreamPlatformName = Platform.GetFriendlyName(),
                    IsLive = true,
                    Views = s.watchingCount,

                }),
                NextPageToken = GetNextPageToken(response.data.livestreams.list.Any(), pageSize, pageOffset),
            };
        }

        public async Task<StreamerChannelDto> GetStreamerChannel(string channelName)
        {
            var response = await dliveApi.GetUserByDisplayName(channelName);

            if (response.data.userByDisplayName == null)
            {
                return null;
            }

            if (!response.data.userByDisplayName.displayName.Equals(channelName, System.StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }

            return new StreamerChannelDto
            {
                ChannelName = response.data.userByDisplayName.displayName,
                AvatarUrl = response.data.userByDisplayName.avatar,
                ChannelUrl = urlBuilder.Build(response.data.userByDisplayName.displayName),
                Platform = Platform,
            };
        }

        public StreamPlatformType Platform => StreamPlatformType.DLive;
    }
}
