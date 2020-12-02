using System;
using GameStreamSearch.Api.Contracts;
using GameStreamSearch.Api.Controllers;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;
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

        public IActionResult PresentChannelAdded(Channel channel)
        {
            var urlParams = new GetChannelParams
            {
                Channel = channel.ChannelName,
                Platform = channel.StreamPlatform,
            };

            var channelDto = ChannelDto.FromEntity(channel);

            return new CreatedResult(controller.Url.Link(nameof(controller.GetChannel), urlParams), channelDto);
        }

        public IActionResult PresentChannelNotFoundOnPlatform(string channelName, StreamPlatformType platform)
        {
            var errorResponse = new ErrorResponseContract()
                .AddError(new ErrorContract
                {
                    ErrorCode = ErrorCodeType.ChannelNotFoundOnPlatform,
                    ErrorMessage = $"Channel {channelName} not found on {platform.GetFriendlyName()}"
                });

            return new BadRequestObjectResult(errorResponse);
        }

        public IActionResult PresentChannelUpdated(Channel channel)
        {
            var channelDto = ChannelDto.FromEntity(channel);

            return new OkObjectResult(channelDto);
        }
    }
}
