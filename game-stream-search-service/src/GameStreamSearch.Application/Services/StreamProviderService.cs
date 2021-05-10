using System;
using System.Collections.Generic;
using GameStreamSearch.Application.StreamProvider;

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

        public IStreamProvider GetProviderByName(string providerName)
        {
            return streamProviders[providerName];
        }

        public IEnumerable<IStreamProvider> StreamProviders => streamProviders.Values;
    }
}
