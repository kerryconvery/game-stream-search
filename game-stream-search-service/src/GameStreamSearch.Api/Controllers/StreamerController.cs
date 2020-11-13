using System.Threading.Tasks;
using GameStreamSearch.Api.Presenters;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GameStreamSearch.Api.Controllers
{
    public class GetStreamerByIdParams
    {
        public string Id { get; set; }
    }

    [ApiController]
    [Route("api")]
    public class StreamerController : ControllerBase
    {
        private readonly IInteractor<StreamerDto, IRegisterStreamerPresenter> registerStreamerInteractor;
        private readonly IInteractor<string, IGetStreamerByIdPresenter> getStreamerByIdInteractor;
        private readonly IStreamerRepository streamerRepository;
        private readonly ITimeProvider timeProvider;
        private readonly IIdProvider idProvider;

        public StreamerController(
            IInteractor<StreamerDto, IRegisterStreamerPresenter> registerStreamerInteractor,
            IInteractor<string, IGetStreamerByIdPresenter> getStreamerByIdInteractor,
            IStreamerRepository streamerRepository,
            ITimeProvider timeProvider,
            IIdProvider idProvider)
        {
            this.registerStreamerInteractor = registerStreamerInteractor;
            this.getStreamerByIdInteractor = getStreamerByIdInteractor;
            this.streamerRepository = streamerRepository;
            this.timeProvider = timeProvider;
            this.idProvider = idProvider;
        }

        [HttpPost]
        [Route("streamers")]
        public async Task<IActionResult> RegisterStreamer([FromBody] RegisterStreamerDto streamer)
        {
            var streamerDto = new StreamerDto
            {
                Id = idProvider.GetNextId(),
                Name = streamer.Name,
                Platform = streamer.Platform,
                DateRegistered = timeProvider.GetNow(),
            };

            var presenter = new RegisterStreamerPresenter(Url, this);

            await registerStreamerInteractor.Invoke(streamerDto, presenter);

            return presenter.Result;
        }

        [HttpGet]
        [Route("streamers")]
        public async Task<IActionResult> GetStreamers()
        {
            var streamers = await streamerRepository.GetStreamers();

            return Ok(streamers);
        }

        [HttpGet]
        [Route("streamers/{id}", Name = "GetStreamerById")]
        public async Task<IActionResult> GetStreamerById([FromRoute] string id)
        {
            var presenter = new GetStreamerByIdPresenter();

            await getStreamerByIdInteractor.Invoke(id, presenter);

            return presenter.Result;
        }
    }
}
