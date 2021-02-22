using System;
using System.Collections.Generic;

namespace GameStreamSearch.Application.Types
{
    public class ChannelListDto
    {
        public IEnumerable<ChannelDto> Items { get; init; }
    }
}
