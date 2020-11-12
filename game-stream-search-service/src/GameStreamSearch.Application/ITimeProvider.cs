using System;
namespace GameStreamSearch.Application
{
    public interface ITimeProvider
    {
        DateTime GetNow();
    }
}
