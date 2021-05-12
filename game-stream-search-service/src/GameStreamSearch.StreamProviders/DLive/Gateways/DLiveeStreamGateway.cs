using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.DLive.Gateways.Dto;

namespace GameStreamSearch.StreamProviders.DLive.Gateways
{
    public interface StreamGateway
    {
        public Task<IEnumerable<DLiveStreamItemDto>> GetLiveStreams(int pageSize, int pageOffset, StreamSortOrder sortOrder);
    }
}
