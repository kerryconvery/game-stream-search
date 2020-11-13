using System;
namespace GameStreamSearch.Application.Providers
{
    public class GuidIdProvider : IIdProvider
    {
        public string GetNextId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
