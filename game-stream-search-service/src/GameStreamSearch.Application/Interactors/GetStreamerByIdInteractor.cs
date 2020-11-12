using System;
using System.Threading.Tasks;

namespace GameStreamSearch.Application.Interactors
{
    public class GetStreamerByIdInteractor : IInteractor<string, IGetStreamerByIdPresenter>
    {
        private readonly IStreamerRepository streamerRepository;

        public GetStreamerByIdInteractor(IStreamerRepository streamerRepository)
        {
            this.streamerRepository = streamerRepository;
        }

        public async Task Invoke(string streamerId, IGetStreamerByIdPresenter presenter)
        {
            var streamer = await streamerRepository.GetStreamerById(streamerId);

            if (streamer != null)
            {
                presenter.PresentStreamer(streamer);
            } else
            {
                presenter.PresentStreamerNotFound();
            }
        }
    }
}
