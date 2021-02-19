using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Base64Url;
using GameStreamSearch.Application.Dto;
using Newtonsoft.Json;

namespace GameStreamSearch.Application.Services
{
    public class StreamAggregationService
    {
        private string PackPageTokens(Dictionary<string, string> paginations)
        {
            if (!paginations.Any())
            {
                return string.Empty;
            }

            var jsonTokens = JsonConvert.SerializeObject(paginations);

            var base64Encryptor = new Base64Encryptor(new ToBase64Transform());

            base64Encryptor.WriteVar(jsonTokens);

            return base64Encryptor.ToString();
        }

        public PlatformStreamsDto AggregateStreams(IEnumerable<PlatformStreamsDto> platformStreams)
        {
            var aggregatedStreams = platformStreams.SelectMany(s => s.Streams);
            var providerPageTokens = platformStreams.ToDictionary(s => s.StreamPlatformId, s => s.NextPageToken);
            var packedPageTokens = PackPageTokens(providerPageTokens);

            return new PlatformStreamsDto(aggregatedStreams, packedPageTokens);
        }

        public Dictionary<string, string> UnpackPageToken(string packedPageTokens)
        {
            if (string.IsNullOrEmpty(packedPageTokens))
            {
                return new Dictionary<string, string>();
            }

            var base64Decrypter = new Base64Decryptor(packedPageTokens, new FromBase64Transform());

            var jsonTokens = base64Decrypter.ReadVarString();

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonTokens);
        }
    }
}
