using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Base64Url;
using GameStreamSearch.Application.StreamProvider;
using Newtonsoft.Json;

namespace GameStreamSearch.Application.Services.StreamProvider
{
    public class PageTokens
    {
        private List<PageToken> pageTokens = new List<PageToken>();

        public PageTokens AddToken(string streamPlatformname, string token)
        {
            pageTokens.Add(new PageToken(streamPlatformname, token));
            return this;
        }

        public PageTokens AddTokens(IEnumerable<PageToken> tokens)
        {
            pageTokens.AddRange(tokens);
            return this;
        }

        public PageToken GetTokenOrEmpty(string streamPlatformName)
        {
            return pageTokens.SingleOrDefault(t => t.StreamPlatformName == streamPlatformName) ?? PageToken.Empty(streamPlatformName);
        }

        public string PackTokens()
        {
            var tokenList = pageTokens
                .Where(t => !t.IsEmpty())
                .ToDictionary(t => t.StreamPlatformName, t => t.Token);

            if (!tokenList.Any())
            {
                return string.Empty;
            }

            var jsonTokens = JsonConvert.SerializeObject(tokenList);

            var base64Encryptor = new Base64Encryptor(new ToBase64Transform());

            base64Encryptor.WriteVar(jsonTokens);

            return base64Encryptor.ToString();
        }

        public static PageTokens UnpackTokens(string packedTokens)
        {
            if (string.IsNullOrEmpty(packedTokens))
            {
                return new PageTokens();
            }

            var base64Decrypter = new Base64Decryptor(packedTokens, new FromBase64Transform());

            var jsonTokens = base64Decrypter.ReadVarString();

            var tokenList = JsonConvert
                .DeserializeObject<Dictionary<string, string>>(jsonTokens)
                .Select(token => new PageToken(token.Key, token.Value));

            return FromList(tokenList);
        }

        public static PageTokens FromList(IEnumerable<PageToken> tokens)
        {
            return new PageTokens().AddTokens(tokens);
        }
    }
}
