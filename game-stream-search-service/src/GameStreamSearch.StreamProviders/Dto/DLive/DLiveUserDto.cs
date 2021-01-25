using System;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.StreamProviders.Dto.DLive
{
    public class DLiveUserDto
    {
        public string displayName { get; set; }
        public string avatar { get; set; }

        public StreamerChannelDto ToStreamerChannelDto(string dliveWebUrl)
        {
            return new StreamerChannelDto
            {
                ChannelName = displayName,
                AvatarUrl = avatar,
                ChannelUrl = $"{dliveWebUrl}/{displayName}",
                Platform = StreamPlatformType.DLive,
            };
        }
    }
}
