using System;
using System.Collections.Generic;

namespace GameStreamSearch.Application.Dto
{
    public class ChannelListDto
    {
        public IEnumerable<ChannelDto> Items { get; init; }
    }
}
