using System;
using System.Collections.Generic;

namespace GameStreamSearch.Application.Models
{
    public class ChannelListDto
    {
        public IEnumerable<ChannelDto> Channels { get; init; }
    }
}
