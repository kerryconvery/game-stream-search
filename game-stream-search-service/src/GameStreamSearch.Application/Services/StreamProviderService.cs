using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Application.StreamProvider;
using GameStreamSearch.Common;

namespace GameStreamSearch.Application.Services
{
    public class StreamProviderService
    {
        private Dictionary<string, IStreamProvider> streamProviders;

        public StreamProviderService()
        {
            streamProviders = new Dictionary<string, IStreamProvider>(StringComparer.OrdinalIgnoreCase);
        }

        public StreamProviderService RegisterStreamProvider(IStreamProvider streamProvider)
        {
            streamProviders.Add(streamProvider.StreamPlatformName, streamProvider);

            return this;
        }

        public IEnumerable<string> GetSupportingPlatforms(StreamFilterOptions streamFilterOptions)
        {
            return streamProviders
                .Values
                .Where(p => p.AreFilterOptionsSupported(streamFilterOptions))
                .Select(p => p.StreamPlatformName);
        }

        public IStreamProvider GetProviderByName(string providerName)
        {
            return streamProviders[providerName];
        }
    }
}
