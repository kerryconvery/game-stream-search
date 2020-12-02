using System;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GameStreamSearch.Api.Presenters
{
    public class GetChannelPresenter : IGetChannelPresenter<IActionResult>
    {
        public IActionResult PresentChannelNotFound()
        {
            return new NotFoundResult();
        }

        public IActionResult PresentChannel(Channel platformChannel)
        {
           return new OkObjectResult(platformChannel);
        }
    }
}
