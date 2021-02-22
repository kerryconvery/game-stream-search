using System;
using Amazon.DynamoDBv2.DataModel;
using GameStreamSearch.Application.Models;

namespace GameStreamSearch.Repositories.Dto
{
    [DynamoDBTable("Channels")]
    public class DynamoDbChannelDto
    {
        [DynamoDBHashKey]
        public string StreamPlatformId { get; init; }

        [DynamoDBRangeKey]
        public string ChannelName { get; init; }

        [DynamoDBProperty]
        public DateTime DateRegistered { get; init; }

        [DynamoDBProperty]
        public string AvatarUrl { get; init; }

        [DynamoDBProperty]
        public string ChannelUrl { get; init; }

        public static DynamoDbChannelDto FromEntity(Channel channel)
        {
            return new DynamoDbChannelDto
            {
                ChannelName = channel.ChannelName,
                StreamPlatformId = channel.StreamPlatformId,
                DateRegistered = channel.DateRegistered,
                AvatarUrl = channel.AvatarUrl,
                ChannelUrl = channel.ChannelUrl,
            };
        }

        public Channel ToEntity()
        {
            return new Channel(ChannelName, StreamPlatformId, DateRegistered, AvatarUrl, ChannelUrl);
        }
    }
}
