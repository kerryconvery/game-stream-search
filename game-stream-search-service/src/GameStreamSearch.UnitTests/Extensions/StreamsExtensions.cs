using System;
using System.Linq;
using GameStreamSearch.Application.ValueObjects;

namespace GameStreamSearch.UnitTests.Extensions
{
    public static class StreamsExtensions
    {
        public static bool IsEmpty(this Streams streams)
        {
            return streams.Items.Count() == 0;
        }
    }
}
