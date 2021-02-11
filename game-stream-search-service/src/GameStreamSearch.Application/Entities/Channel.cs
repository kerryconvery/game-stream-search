using System;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.Entities
{
    public class Channel
    {
        public Channel(string channelName, StreamPlatformType streamPlatform, DateTime dateRegistered, string avatarUrl, string channelUrl)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentException("Channel name cannot be empty or null");
            }

            ChannelName = channelName;
            StreamPlatform = streamPlatform;
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
        public StreamPlatformType StreamPlatform { get; }
        public DateTime DateRegistered { get; }
        public string AvatarUrl { get; private set; }
        public string ChannelUrl { get; private set; }
    }
}
