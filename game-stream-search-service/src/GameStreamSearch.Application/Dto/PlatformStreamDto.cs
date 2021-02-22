using System;
using System.Collections.Generic;

namespace GameStreamSearch.Application.Types
{
    public class PlatformStreamDto : ValueObject
    {
        public string StreamTitle { get; init; }
        public string StreamThumbnailUrl { get; init; }
        public string StreamUrl { get; init; }
        public string StreamerName { get; init; }
        public string StreamerAvatarUrl { get; init; }
        public bool IsLive { get; init; }
        public int Views { get; init; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StreamUrl;
        }
    }
}
