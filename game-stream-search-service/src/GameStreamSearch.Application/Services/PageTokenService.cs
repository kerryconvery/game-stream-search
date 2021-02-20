using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Base64Url;
using Newtonsoft.Json;

namespace GameStreamSearch.Application.Services
{
    public class PageTokenService
    {
        public string PackPageTokens(Dictionary<string, string> paginations)
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
