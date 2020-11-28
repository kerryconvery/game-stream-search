using System;
using GameStreamSearch.Api.Controllers;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Interactors;
using Microsoft.AspNetCore.Mvc;

namespace GameStreamSearch.Api.Presenters
{
    public class UpsertChannelPresenter : IUpsertChannelPresenter<IActionResult>
    {
        private readonly ChannelsController controller;

        public UpsertChannelPresenter(ChannelsController controller)
        {
            this.controller = controller;
        }

        public IActionResult PresentChannelAdded(string channelName, StreamPlatformType streamPlatform)
        {
            var urlParams = new GetChannelParams
            {
                Channel = channelName,
                Platform = streamPlatform,
            };

            return new CreatedResult(controller.Url.Link(nameof(controller.GetChannel), urlParams), null);
        }

        public IActionResult PresentChannelNotFoundOnPlatform(string channelName, StreamPlatformType platform)
        {
            return new BadRequestObjectResult($"A channel for {channelName} was not found on { platform.GetFriendlyName()}");
        }

        public IActionResult PresentChannelUpdated()
        {
            return new NoContentResult();
        }
    }
}
