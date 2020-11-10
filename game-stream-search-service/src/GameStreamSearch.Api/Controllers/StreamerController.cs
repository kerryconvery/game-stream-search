using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Api.Dto;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GameStreamSearch.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class StreamerController : ControllerBase
    {
        private readonly IStreamerRepository streamerRepository;

        public StreamerController(IStreamerRepository streamerRepository)
        {
            this.streamerRepository = streamerRepository;
        }

        [HttpPost]
        [Route("streamers")]
        public async Task<IActionResult> RegisterStreamer([FromBody] RegisterStreamerDto streamer)
        {
            return Created("", 1);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StreamerDto>>> GetStreamers()
        {
            return new List<StreamerDto>();
        }
    }
}
