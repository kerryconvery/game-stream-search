using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application
{
    public class UpsertChannelRequest
    {
        public string ChannelName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public StreamPlatformType StreamPlatform { get; set; }
    }

    public interface IUpsertChannelPresenter<Result>
    {
        Result PresentChannelAdded(Channel channel);
        Result PresentChannelUpdated(Channel channel);
        Result PresentChannelNotFoundOnPlatform(string channelName, StreamPlatformType platform);
    }

    public interface IUpsertChannel
    {
        Task<Result> Invoke<Result>(UpsertChannelRequest request, IUpsertChannelPresenter<Result> presenter);
    }
}
