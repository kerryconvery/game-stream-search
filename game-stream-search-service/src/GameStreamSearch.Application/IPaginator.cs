using System;
using System.Collections.Generic;

namespace GameStreamSearch.Application
{
    public interface IPaginator
    {
        string encode(Dictionary<string, string> paginations);
        Dictionary<string, string> decode(string encodedPaginations);
    }
}
