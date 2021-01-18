using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application
{
    public enum UpsertChannelResult
    {
        ChannelNotFoundOnPlatform,
        ChannelAdded,
        ChannelUpdated,
        PlatformServiceIsNotAvailable,
    }

    public class UpsertChannelRequest
    {
        public string ChannelName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public StreamPlatformType StreamPlatform { get; set; }
    }

    public interface IUpsertChannelCommand
    {
        Task<UpsertChannelResult> Invoke(UpsertChannelRequest request);
    }
}
