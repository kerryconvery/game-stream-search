using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.Interactors
{
    public class GetChannelInteractor : IGetChannel
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
