using System;

namespace GameStreamSearch.StreamProviders.DLive.Gateways.Dto
{
    public class DLiveUserByDisplayNameDataDto
    {
        public DLiveUserDto userByDisplayName { get; set; }
    }

    public class DLiveUserByDisplayNameDto
    {
        public DLiveUserByDisplayNameDataDto data { get; set; }
    }
}
