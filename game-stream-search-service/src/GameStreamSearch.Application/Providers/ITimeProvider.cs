using System;
namespace GameStreamSearch.Application.Providers
{
    public interface ITimeProvider
    {
        DateTime GetNow();
    }
}
