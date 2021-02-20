using System;
using System.Collections.Generic;

namespace GameStreamSearch.Application.ValueObjects
{
    public class StreamPlatform : ValueObject
    {
        public StreamPlatform(string platformId, string platformDisplayName)
        {
            PlatformId = platformId;
            PlatformDisplayName = platformDisplayName;
        }

        public string PlatformId { get; }
        public string PlatformDisplayName { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PlatformId;
        }
    }
}
