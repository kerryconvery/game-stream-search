namespace GameStreamSearch.Application.Types
{
    public class ChannelDto
    {
        public string ChannelName { get; init; }
        public string StreamPlatformId { get; init; }
        public string AvatarUrl { get; init; }
        public string ChannelUrl { get; init; }

        public static ChannelDto FromEntity(Channel channel)
        {
            return new ChannelDto
            {
                ChannelName = channel.ChannelName,
                StreamPlatformId = channel.StreamPlatformId,
                AvatarUrl = channel.AvatarUrl,
                ChannelUrl = channel.ChannelUrl,
            };
        }
    }
}
