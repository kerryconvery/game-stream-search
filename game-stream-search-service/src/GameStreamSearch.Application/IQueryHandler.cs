using System;
using System.Threading.Tasks;

namespace GameStreamSearch.Application
{
    public interface IQueryHandler<TQuery, TResult>
    {
        Task<TResult> Execute(TQuery query);
    }
}
