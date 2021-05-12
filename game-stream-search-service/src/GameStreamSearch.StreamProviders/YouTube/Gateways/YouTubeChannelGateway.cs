using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.YouTube.Gateways.Dto.V3;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.YouTube.Gateways
{
    public interface YouTubeChannelGateway
    {
        Task<Maybe<YouTubeChannelDto>> GetChannelByName(string channelNam);
        Task<IEnumerable<YouTubeChannelDto>> BulkGetAvartarByChannelId(string[] channelIds);
    }
}
