using System;
namespace GameStreamSearch.Application.Providers
{
    public class IdProvider : IIdProvider
    {
        public string GetNextId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
