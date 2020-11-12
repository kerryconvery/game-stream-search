using System;
using System.Linq;
using System.Collections.Generic;
using GameStreamSearch.Services.Interfaces;
using Base64Url;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;

namespace GameStreamSearch.Api.Infrastructor
{
    public class Paginator : IPaginator
    {
        public Paginator()
        {
        }

        public Dictionary<string, string> decode(string encodedPaginations)
        {
            if (string.IsNullOrEmpty(encodedPaginations))
            {
                return new Dictionary<string, string>();
            }

            var base64Decrypter = new Base64Decryptor(encodedPaginations, new FromBase64Transform());

            var jsonTokens = base64Decrypter.ReadVarString();

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonTokens);
        }

        public string encode(Dictionary<string, string> paginations)
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
    }
}
