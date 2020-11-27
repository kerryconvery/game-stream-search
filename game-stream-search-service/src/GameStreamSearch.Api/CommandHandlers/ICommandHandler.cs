using System;
using System.Threading.Tasks;

namespace GameStreamSearch.Api.CommandHandlers
{
    public interface ICommandHandler<T>
    {
        public Task Handle(T command);
    }
}
