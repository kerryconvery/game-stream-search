using System;
using System.Threading.Tasks;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application
{
    public interface ICommandHandler<TCommand, TError>
    {
        Task<Result<TError>> Handle(TCommand request);
    }
}
