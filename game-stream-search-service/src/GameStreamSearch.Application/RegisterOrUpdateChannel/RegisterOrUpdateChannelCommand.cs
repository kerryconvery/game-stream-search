using System;
namespace GameStreamSearch.Domain.Commands
{
    public class RegisterOrUpdateChannelCommand
    {
        public string ChannelName { get; init; }
        public DateTime RegistrationDate { get; init; }
        public string StreamPlatformName { get; init; }
    }
}
