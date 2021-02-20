using System;
using System.Linq;
using GameStreamSearch.Application.ValueObjects;

namespace GameStreamSearch.UnitTests.Extensions
{
    public static class StreamsExtensions
    {
        public static bool IsEmpty(this PlatformStreams streams)
        {
            return streams.Streams.Count() == 0;
        }
    }
}
