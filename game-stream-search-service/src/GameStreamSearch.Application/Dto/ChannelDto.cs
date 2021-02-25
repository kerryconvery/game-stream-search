using GameStreamSearch.Domain.Entities;

namespace GameStreamSearch.Application.Dto
{
    public class ChannelDto
    {
        public string ChannelName { get; init; }
        public string PlatformName { get; init; }
        public string AvatarUrl { get; init; }
        public string ChannelUrl { get; init; }

        public static ChannelDto FromEntity(Channel channel)
        {
            return new ChannelDto
            {
                ChannelName = channel.ChannelName,
                PlatformName = channel.StreamPlatformName,
                AvatarUrl = channel.AvatarUrl,
                ChannelUrl = channel.ChannelUrl,
            };
        }
    }
}
