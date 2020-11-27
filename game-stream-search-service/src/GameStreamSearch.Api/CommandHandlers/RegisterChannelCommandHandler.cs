using System;
using System.Threading.Tasks;
using GameStreamSearch.Api.Repositories;
using GameStreamSearch.Api.Services;
using GameStreamSearch.Domain.Commands;

namespace GameStreamSearch.Api.CommandHandlers
{
    public class RegisterChannelCommandHandler : ICommandHandler<RegisterChannelCommand>
    {
        public RegisterChannelCommandHandler(IChannelRepository channelRepository, StreamService streamService)
        {

        }

        public async Task Handle(RegisterChannelCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
