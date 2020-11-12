
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Exceptions;

namespace GameStreamSearch.Application.Interactors
{
    public class RegisterStreamerInteractor : IInteractor<StreamerDto, IRegisterStreamerPresenter>
    {
        private readonly IStreamerRepository streamerRepository;
        private readonly IStreamService streamService;

        public RegisterStreamerInteractor(
            IStreamerRepository streamerRepository,
            IStreamService streamService)
        {
            this.streamerRepository = streamerRepository;
            this.streamService = streamService;
        }

        public async Task Invoke(StreamerDto streamer, IRegisterStreamerPresenter presenter)
        {
            StreamerChannelDto streamerChannel;

            try
            {
                streamerChannel = await streamService.GetStreamerChannel(streamer.Name, streamer.Platform);

                if (streamerChannel == null)
                {
                    presenter.PresentStreamerDoesNotHaveAChannel(streamer.Name, streamer.Platform);

                    return;
                }
            } catch(StreamProviderUnavailableException)
            {
            }

            var existingStreamer = await streamerRepository.GetStreamerByNameAndPlatform(streamer.Name, streamer.Platform);

            if (existingStreamer != null)
            {
                presenter.PresentStreamerRegistered(existingStreamer.Id);

                return;
            }

            await streamerRepository.SaveStreamer(streamer);

            presenter.PresentStreamerRegistered(streamer.Id);
        }
    }
}
