namespace GameStreamSearch.Application.StreamProvider
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
}
