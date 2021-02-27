using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Base64Url;
using Newtonsoft.Json;

namespace GameStreamSearch.Application.Models
{
    public class PageToken
    {
        public PageToken(string streamPlatformName, string token)
        {
            StreamPlatformName = streamPlatformName;
            Token = token;
        }

        public string StreamPlatformName { get; }
        public string Token { get; }

        public bool IsEmpty() => string.IsNullOrEmpty(Token);
        public static PageToken Empty(string platformName) => new PageToken(platformName, string.Empty);
        public static implicit operator int(PageToken pageToken)
        {
            int numericToken;

            if (int.TryParse(pageToken.Token, out numericToken))
            {
                return numericToken;
            }
            return 0;
        }

        public static implicit operator string(PageToken pageToken) => pageToken.Token;
    }

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
