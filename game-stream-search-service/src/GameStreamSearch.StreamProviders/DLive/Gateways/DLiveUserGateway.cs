using System;
using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.DLive.Gateways.Dto;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.DLive.Gateways
{
    public interface DLiveUserGateway
    {
        Task<Maybe<DLiveUserDto>> GetUserByDisplayName(string displayName)
    }
}
