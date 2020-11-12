using System;
namespace GameStreamSearch.Application.Providers
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime GetNow()
        {
            return DateTime.Now.ToUniversalTime();
        }
    }
}
