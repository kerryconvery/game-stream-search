using System;
using GameStreamSearch.Api.Controllers;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GameStreamSearch.Api.Presenters
{
    public class RegisterStreamerPresenter : IRegisterStreamerPresenter
    {
        private readonly IUrlHelper urlHelper;
        private readonly StreamerController controller;

        public RegisterStreamerPresenter(IUrlHelper urlHelper, StreamerController controller)
        {
            this.urlHelper = urlHelper;
            this.controller = controller;
        }

        public void PresentStreamerRegistered(string streamerId)
        {
            Result = new CreatedResult(urlHelper.Link(nameof(controller.GetStreamer), new GetStreamerByIdParams { Id = streamerId }), null);
        }

        public void PresentStreamerDoesNotHaveAChannel(string streamerName, StreamingPlatform platform)
        {

            Result = new BadRequestObjectResult($"A channel for {streamerName} was not found on {platform.GetFriendlyName()}");
        }

        public IActionResult Result { get; private set; }
    }
}
