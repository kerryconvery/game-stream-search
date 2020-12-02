using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application
{
    public interface IGetChannelPresenter<Result>
    {
        Result PresentChannel(Channel platformChannel);
        Result PresentChannelNotFound();
    }

    public interface IGetChannel
    {
        Task<Result> Invoke<Result>(StreamPlatformType streamPlatform, string channelName, IGetChannelPresenter<Result> presenter);
    }
}
