using System;
using Amazon.DynamoDBv2.DataModel;

namespace GameStreamSearch.DataAccess.Dto
{
    [DynamoDBTable("Channels")]
    public class ChannelTableDto
    {
        [DynamoDBHashKey]
        public string StreamPlatformName { get; init; }

        [DynamoDBRangeKey]
        public string ChannelName { get; init; }

        [DynamoDBProperty]
        public DateTime DateRegistered { get; init; }

        [DynamoDBProperty]
        public string AvatarUrl { get; init; }

        [DynamoDBProperty]
        public string ChannelUrl { get; init; }
    }
}
