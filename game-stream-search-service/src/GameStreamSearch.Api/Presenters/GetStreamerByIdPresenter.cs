using System;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GameStreamSearch.Api.Presenters
{
    public class GetStreamerByIdPresenter : IGetStreamerByIdPresenter
    {
        public void PresentStreamer(StreamerDto streamer)
        {
            Result = new OkObjectResult(streamer);
        }

        public void PresentStreamerNotFound()
        {
            Result = new NotFoundResult();
        }

        public IActionResult Result { get; set; }
    }
}
