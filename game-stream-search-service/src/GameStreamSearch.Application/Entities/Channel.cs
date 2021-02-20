using System;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.ValueObjects
{
    public class Channel
    {
        public Channel(string channelName, string streamPlatformId, DateTime dateRegistered, string avatarUrl, string channelUrl)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentException("Channel name cannot be empty or null");
            }

            ChannelName = channelName;
            StreamPlatformId = streamPlatformId;
            DateRegistered = dateRegistered;

            SetAvatarUrl(avatarUrl);
            SetChannelUrl(channelUrl);
        }

        public void SetAvatarUrl(string avatarUrl)
        {
            if (string.IsNullOrEmpty(avatarUrl))
            {
                throw new ArgumentException("Avatar url cannot be empty or null");
            }

            AvatarUrl = avatarUrl;

        }

        public void SetChannelUrl(string channelUrl)
        {
            if (string.IsNullOrEmpty(channelUrl))
            {
                throw new ArgumentException("Channel url cannot be empty or null");
            }

            ChannelUrl = channelUrl;
        }

        public string ChannelName { get; }
        public string StreamPlatformId { get; }
        public DateTime DateRegistered { get; }
        public string AvatarUrl { get; private set; }
        public string ChannelUrl { get; private set; }
    }
}
