using System;
namespace GameStreamSearch.Common
{
    public class PlatformUnavailableException : Exception
    {
        public PlatformUnavailableException(string platformName)
        {
            PlatformName = platformName;
        }

        public string PlatformName { get; }
    }
}
