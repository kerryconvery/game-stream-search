using System;
using System.Threading.Tasks;

namespace GameStreamSearch.Application
{
    public interface ICommandHandler<TCommand>
    {
        Task Handle(TCommand request);
    }
}
