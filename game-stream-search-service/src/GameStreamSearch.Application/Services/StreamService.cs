﻿using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Base64Url;

namespace GameStreamSearch.Application.Services
{
    public class StreamService : IStreamService
    {
        private Dictionary<StreamPlatformType, IStreamProvider> streamProviders;

        public StreamService()
        {
            streamProviders = new Dictionary<StreamPlatformType, IStreamProvider>();
        }

        private Dictionary<StreamPlatformType, string> UnpackPageTokens(string encodedPaginations)
        {
            if (string.IsNullOrEmpty(encodedPaginations))
            {
                return new Dictionary<StreamPlatformType, string>();
            }

            var base64Decrypter = new Base64Decryptor(encodedPaginations, new FromBase64Transform());

            var jsonTokens = base64Decrypter.ReadVarString();

            return JsonConvert.DeserializeObject<Dictionary<StreamPlatformType, string>>(jsonTokens);
        }

        private string PackPageTokens(Dictionary<StreamPlatformType, string> paginations)
        {
            if (!paginations.Any())
            {
                return null;
            }

            var jsonTokens = JsonConvert.SerializeObject(paginations);

            var base64Encryptor = new Base64Encryptor(new ToBase64Transform());

            base64Encryptor.WriteVar(jsonTokens);

            return base64Encryptor.ToString();
        }

        private string AggregateNextPageTokens(GameStreamsDto[] gameStreams)
        {
            var pageTokens = new Dictionary<StreamPlatformType, string>();

            for (int index = 0; index < gameStreams.Length; index++)
            {
                if (gameStreams[index].NextPageToken != null)
                {
                    pageTokens.Add(streamProviders.Values.ElementAt(index).Platform, gameStreams[index].NextPageToken);
                }
            }

            return PackPageTokens(pageTokens);
        }

        public async Task<GameStreamsDto> GetStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken)
        {
            var paginationTokens = UnpackPageTokens(pageToken);

            var tasks = streamProviders.Values.Select(p => {
                var pageToken = paginationTokens.ContainsKey(p.Platform) ? paginationTokens[p.Platform] : null;

                return p.GetLiveStreams(filterOptions, pageSize, pageToken);
            });

            var results = await Task.WhenAll(tasks);

            var nextPageToken = AggregateNextPageTokens(results);

            var sortedItems = results
                .SelectMany(s => s.Items)
                .OrderByDescending(s => s.Views);

            return new GameStreamsDto()
            {
                Items = sortedItems,
                NextPageToken = nextPageToken
            };
        }

        public StreamService RegisterStreamProvider(IStreamProvider streamProvider)
        {
            streamProviders.Add(streamProvider.Platform, streamProvider);

            return this;
        }

        public Task<StreamerChannelDto> GetStreamerChannel(string streamerName, StreamPlatformType streamingPlatform)
        {
            var streamProvider = streamProviders[streamingPlatform];

            return streamProvider.GetStreamerChannel(streamerName);
        }

        public IEnumerable<StreamPlatformDto> GetStreamAllPlatforms()
        {
            return streamProviders.Keys.Select(k => new StreamPlatformDto { StreamPlatform = k });
        }
    }
}
