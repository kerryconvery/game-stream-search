using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Api.Controllers;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Commands;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Providers;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Types;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace GameStreamSearch.Api.Tests
{
    public class ChannelControllerTests
    {
        private ChannelsController channelController;
        private Mock<ITimeProvider> timeProviderStub = new Mock<ITimeProvider>();
        private Mock<IStreamProvider> youTubeStreamProviderStub = new Mock<IStreamProvider>();
        private Mock<IChannelRepository> channelRepositoryMock = new Mock<IChannelRepository>();

        private readonly DateTime registrationDate = DateTime.Now;
        private List<Channel> dataStore = new List<Channel>();

        private readonly StreamerChannelDto youtubeChannelDto = new StreamerChannelDto
        {
            ChannelName = "Youtube channel",
            Platform = StreamPlatformType.YouTube,
            AvatarUrl = "avatar url",
            ChannelUrl = "channel url",
        };

        [SetUp]
        public void Setup()
        {
            channelRepositoryMock.Setup(s => s.Add(It.IsAny<Channel>())).Callback<Channel>(channel => dataStore.Add(channel));

            youTubeStreamProviderStub.SetupGet(s => s.Platform).Returns(StreamPlatformType.YouTube);
            youTubeStreamProviderStub.Setup(s => s.GetStreamerChannel(youtubeChannelDto.ChannelName)).ReturnsAsync(
                MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Success(youtubeChannelDto)
            );

            ProviderAggregationService streamService = new ProviderAggregationService()
                .RegisterStreamProvider(youTubeStreamProviderStub.Object);

            timeProviderStub.Setup(s => s.GetNow()).Returns(registrationDate);

            var upsertChannelCommand = new UpsertChannelCommand(channelRepositoryMock.Object, streamService);

            channelController = new ChannelsController(
                upsertChannelCommand,
                channelRepositoryMock.Object,
                timeProviderStub.Object);

            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();

            urlHelper.Setup(s => s.Link(nameof(channelController.GetChannel), It.IsAny<object>()))
                .Returns<string, GetChannelParams>((routeName, routeParams) =>
                    {
                        return routeParams.ChannelName;
                    });

            channelController.Url = urlHelper.Object;
        }

        [Test]
        public async Task Should_Add_A_New_Channel()
        {
            dataStore.Clear();

            channelRepositoryMock.Setup(s => s.Add(It.IsAny<Channel>())).Callback<Channel>(channel => dataStore.Add(channel));
            channelRepositoryMock
                .Setup(s => s.Get(StreamPlatformType.YouTube, "Youtube channel"))
                .ReturnsAsync(() => dataStore.Count == 0 ? Maybe<Channel>.Nothing: Maybe<Channel>.Some(dataStore[0]));

            var createResponse = await channelController.AddChannel(StreamPlatformType.YouTube, "Youtube channel");
            var createResult = createResponse as CreatedResult;

            var getResponse = await channelController.GetChannel(StreamPlatformType.YouTube, "Youtube channel");

            var channel = GetResponseValue(getResponse);

            Assert.IsInstanceOf<CreatedResult>(createResponse);
            Assert.AreEqual(createResult.Location, channel.ChannelName);

            Assert.AreEqual(channel.ChannelName, youtubeChannelDto.ChannelName);
            Assert.AreEqual(channel.StreamPlatform, youtubeChannelDto.Platform);
            Assert.AreEqual(channel.AvatarUrl, youtubeChannelDto.AvatarUrl);
            Assert.AreEqual(channel.ChannelUrl, youtubeChannelDto.ChannelUrl);
        }

        [Test]
        public async Task Should_Update_The_Channel_If_The_Channel_Already_Exists_For_The_Platform()
        {
            dataStore.Clear();

            var existingChannel = new Channel
            {
                ChannelName = "Youtube channel",
                StreamPlatform = StreamPlatformType.YouTube,
                AvatarUrl = "old avatar url",
                ChannelUrl = "old channel url",
            };

            var updatedChannelDto = new StreamerChannelDto
            {
                ChannelName = "Youtube channel",
                Platform = StreamPlatformType.YouTube,
                AvatarUrl = "new avatar url",
                ChannelUrl = "new channel url",
            };

            dataStore.Add(existingChannel);

            channelRepositoryMock.Setup(s => s.Get(StreamPlatformType.YouTube, "Youtube channel")).ReturnsAsync(() => Maybe<Channel>.Some(dataStore[0]));
            channelRepositoryMock.Setup(s => s.Update(It.IsAny<Channel>())).Callback<Channel>(channel => dataStore[0] = channel);

            youTubeStreamProviderStub
                .Setup(s => s.GetStreamerChannel(updatedChannelDto.ChannelName))
                .ReturnsAsync(MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Success(updatedChannelDto));

            await channelController.AddChannel(StreamPlatformType.YouTube, "Youtube channel");

            var updateResponse = await channelController.AddChannel(StreamPlatformType.YouTube, "Youtube channel");
            var result = updateResponse as OkObjectResult;

            var getResponse = await channelController.GetChannel(StreamPlatformType.YouTube, "Youtube channel");
            var channel = GetResponseValue(getResponse);

            Assert.AreEqual(dataStore.Count(), 1);
            Assert.IsInstanceOf<NoContentResult>(updateResponse);
            Assert.AreEqual(channel.AvatarUrl, updatedChannelDto.AvatarUrl);
            Assert.AreEqual(channel.ChannelUrl, updatedChannelDto.ChannelUrl);
        }

        private ChannelDto GetResponseValue(IActionResult response)
        {
            var getValue = (response as OkObjectResult).Value;
            return getValue as ChannelDto;
        }
    }
}
