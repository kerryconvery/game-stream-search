using System;
using System.Collections.Generic;

namespace GameStreamSearch.Application.Dto
{
    public class ChannelListDto
    {
        public IEnumerable<ChannelDto> Channels { get; init; }
    }
}
