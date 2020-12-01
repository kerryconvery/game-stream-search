using System;
using Amazon.DynamoDBv2.DataModel;
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
    }
}
