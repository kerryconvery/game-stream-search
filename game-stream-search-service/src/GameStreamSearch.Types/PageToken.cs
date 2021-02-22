namespace GameStreamSearch.Application.Types
{
    public struct PageToken
    {
        public PageToken(string streamPlatformName, string token)
        {
            StreamPlatformName = streamPlatformName;
            Token = token;
        }

        public string StreamPlatformName { get; }
        public string Token { get; }
    }
}
