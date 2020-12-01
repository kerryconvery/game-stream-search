using System;
using Amazon.DynamoDBv2.DataModel;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Repositories.AwsDynamoDbRepositories.Dto
{
    [DynamoDBTable("Channels")]
    public class DynamoDbChannelDto
    {
        [DynamoDBHashKey]
        public string ChannelName { get; init; }

        [DynamoDBRangeKey]
        public StreamPlatformType StreamPlatform { get; init; }

        [DynamoDBProperty]
        public DateTime DateRegistered { get; set; }

        [DynamoDBProperty]
        public string AvatarUrl { get; set; }

        [DynamoDBProperty]
        public string ChannelUrl { get; set; }

        public static DynamoDbChannelDto FromEntity(Channel channel)
        {
            return new DynamoDbChannelDto
            {
                ChannelName = channel.ChannelName,
                StreamPlatform = channel.StreamPlatform,
                DateRegistered = channel.DateRegistered,
                AvatarUrl = channel.AvatarUrl,
                ChannelUrl = channel.ChannelUrl,
            };
        }

        public Channel ToEntity()
        {
            return new Channel
            {
                ChannelName = ChannelName,
                StreamPlatform = StreamPlatform,
                DateRegistered = DateRegistered,
                AvatarUrl = AvatarUrl,
                ChannelUrl = ChannelUrl,
            };
        }
    }
}
