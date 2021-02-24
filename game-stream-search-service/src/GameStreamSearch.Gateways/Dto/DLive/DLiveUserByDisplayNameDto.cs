using System;

namespace GameStreamSearch.Application.Dto.DLive
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
