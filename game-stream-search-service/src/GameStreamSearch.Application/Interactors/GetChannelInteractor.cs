using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Repositories;

namespace GameStreamSearch.Application.Interactors
{
    public interface IGetChannelPresenter<Result>
    {
        Result PresentChannel(Channel platformChannel);
        Result PresentChannelNotFound();
    }

    public class GetChannelInteractor
    {
        private readonly IChannelRepository channelRepository;

        public GetChannelInteractor(IChannelRepository channelRepository)
        {
            this.channelRepository = channelRepository;
        }

        public async Task<Result> Invoke<Result>(StreamPlatformType streamPlatform, string channelName, IGetChannelPresenter<Result> presenter)
        {
            var channel = await channelRepository.Get(streamPlatform, channelName);

            if (channel != null)
            {
                return presenter.PresentChannel(channel);
            }

            return presenter.PresentChannelNotFound();
        }
    }
}
