using System;
using System.Threading.Tasks;

namespace GameStreamSearch.Application
{
    public interface IInteractor<R, P>
    {
        Task Invoke(R request, P presenter);
    }
}
