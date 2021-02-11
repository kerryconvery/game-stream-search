using System;
using System.Collections.Generic;

namespace GameStreamSearch.Application.ValueObjects
{
    public class ChannelListDto
    {
        public IEnumerable<ChannelDto> Items { get; init; }
    }
}
