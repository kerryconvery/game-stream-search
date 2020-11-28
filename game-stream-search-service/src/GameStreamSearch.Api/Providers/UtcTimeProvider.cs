using System;
namespace GameStreamSearch.Application.Providers
{
    public class UtcTimeProvider : ITimeProvider
    {
        public DateTime GetNow()
        {
            return DateTime.Now.ToUniversalTime();
        }
    }
}
