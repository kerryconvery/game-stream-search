namespace GameStreamSearch.Application.Types
{
    public class StreamPlatform
    {
        public static StreamPlatform YouTube = new StreamPlatform("YouTube");
        public static StreamPlatform Twitch = new StreamPlatform("Twitch");
        public static StreamPlatform DLive = new StreamPlatform("DLive");

        public StreamPlatform(string platformName)
        {
            Name = platformName;
        }

        public string Name { get; }

        public static implicit operator string(StreamPlatform streamPlatform) => streamPlatform.Name;
        public static implicit operator StreamPlatform(string name) => new StreamPlatform(name);

    }
}
