using System.Linq;
using GameStreamSearch.Application.StreamProvider.Dto;

namespace GameStreamSearch.UnitTests.Extensions
{
    public static class StreamsExtensions
    {
        public static bool IsEmpty(this PlatformStreamsDto streams)
        {
            return streams.Streams.Count() == 0;
        }
    }
}
