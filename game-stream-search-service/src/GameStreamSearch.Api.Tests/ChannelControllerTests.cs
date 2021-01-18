using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Api.Contracts;
using GameStreamSearch.Api.Controllers;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Commands;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Providers;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace GameStreamSearch.Api.Tests
{
    public class ChannelControllerTests
    {
        private ChannelsController channelController;
        private Mock<ITimeProvider> timeProviderStub;
        private Mock<IStreamProvider> youTubeStreamProviderStub;
        private Mock<IStreamProvider> twitchtreamProviderStub;
        private Mock<IChannelRepository> channelRepositoryStub;

        private readonly DateTime registrationDate = DateTime.Now;
        private List<Channel> dataStore;

        private readonly StreamerChannelDto youtubeChannelDto = new StreamerChannelDto
        {
            ChannelName = "Youtube channel",
            Platform = StreamPlatformType.YouTube,
            AvatarUrl = "avatar url",
            ChannelUrl = "channel url",
        };

        private readonly StreamerChannelDto twitchChannelDto = new StreamerChannelDto
        {
            ChannelName = "Twitch channel",
            Platform = StreamPlatformType.Twitch,
            AvatarUrl = "avatar url",
            ChannelUrl = "channel url",
        };

        [SetUp]
        public void Setup()
        {
            dataStore = new List<Channel>();

            channelRepositoryStub = new Mock<IChannelRepository>();
            channelRepositoryStub.Setup(s => s.Add(It.IsAny<Channel>())).Callback<Channel>(channel => dataStore.Add(channel));

            youTubeStreamProviderStub = new Mock<IStreamProvider>();
            youTubeStreamProviderStub.SetupGet(s => s.Platform).Returns(StreamPlatformType.YouTube);
            youTubeStreamProviderStub.Setup(s => s.GetStreamerChannel(youtubeChannelDto.ChannelName)).ReturnsAsync(
                MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Success(youtubeChannelDto)
            );

            twitchtreamProviderStub = new Mock<IStreamProvider>();
            twitchtreamProviderStub.SetupGet(s => s.Platform).Returns(StreamPlatformType.Twitch);
            twitchtreamProviderStub.Setup(s => s.GetStreamerChannel(twitchChannelDto.ChannelName)).ReturnsAsync(
                MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Success(twitchChannelDto));

            StreamService streamService = new StreamService()
                .RegisterStreamProvider(youTubeStreamProviderStub.Object)
                .RegisterStreamProvider(twitchtreamProviderStub.Object);

            timeProviderStub = new Mock<ITimeProvider>();

            timeProviderStub.Setup(s => s.GetNow()).Returns(registrationDate);

            IUpsertChannelCommand upsertChannelCommand = new UpsertChannelCommand(channelRepositoryStub.Object, streamService);

            channelController = new ChannelsController(
                upsertChannelCommand,
                channelRepositoryStub.Object,
                timeProviderStub.Object);

            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();

            urlHelper.Setup(s => s.Link(nameof(channelController.GetChannel), It.IsAny<object>())).Returns<string, GetChannelParams>((routeName, routeParams) =>
            {
                return routeParams.ChannelName;
            });

            channelController.Url = urlHelper.Object;
        }

        [Test]
        public async Task Should_Add_A_New_Channel()
        {
            dataStore.Clear();

            channelRepositoryStub.Setup(s => s.Add(It.IsAny<Channel>())).Callback<Channel>(channel => dataStore.Add(channel));
            channelRepositoryStub
                .Setup(s => s.Get(StreamPlatformType.YouTube, "Youtube channel"))
                .ReturnsAsync(() => dataStore.Count == 0 ? Maybe<Channel>.Nothing() : Maybe<Channel>.Just(dataStore[0]));

            var createResponse = await channelController.AddChannel(StreamPlatformType.YouTube, "Youtube channel");
            var createResult = createResponse as CreatedResult;

            var getResponse = await channelController.GetChannel(StreamPlatformType.YouTube, "Youtube channel");
            var getValue = (getResponse as OkObjectResult).Value;
            var channel = getValue as ChannelDto;


            Assert.IsInstanceOf<CreatedResult>(createResponse);
            Assert.AreEqual(createResult.Location, channel.ChannelName);

            Assert.AreEqual(channel.ChannelName, youtubeChannelDto.ChannelName);
            Assert.AreEqual(channel.StreamPlatform, youtubeChannelDto.Platform);
            Assert.AreEqual(channel.AvatarUrl, youtubeChannelDto.AvatarUrl);
            Assert.AreEqual(channel.ChannelUrl, youtubeChannelDto.ChannelUrl);
        }

        [Test]
        public async Task Should_Allow_Adding_The_Same_Channel_For_Multiple_Platforms()
        {
            dataStore.Clear();

            channelRepositoryStub
                .Setup(s => s.Get(StreamPlatformType.Twitch, "Twitch channel"))
                .ReturnsAsync(() => dataStore.Count <= 0 ? Maybe<Channel>.Nothing() : Maybe<Channel>.Just(dataStore[0]));

            channelRepositoryStub
                .Setup(s => s.Get(StreamPlatformType.YouTube, "Youtube channel"))
                .ReturnsAsync(() => dataStore.Count <= 1 ? Maybe<Channel>.Nothing() : Maybe<Channel>.Just(dataStore[1]));

            await channelController.AddChannel(StreamPlatformType.Twitch, "Twitch channel");
            await channelController.AddChannel(StreamPlatformType.YouTube, "Youtube channel");

            var getTwitchResponse = await channelController.GetChannel(StreamPlatformType.Twitch, "Twitch channel");
            var getTwitchValue = (getTwitchResponse as OkObjectResult).Value;
            var twitchChannel = getTwitchValue as ChannelDto;

            var getYouTubeResponse = await channelController.GetChannel(StreamPlatformType.YouTube, "Youtube channel");
            var getYouTubeValue = (getYouTubeResponse as OkObjectResult).Value;
            var youTubeChannel = getYouTubeValue as ChannelDto;

            Assert.AreEqual(twitchChannel.ChannelName, "Twitch channel");
            Assert.AreEqual(twitchChannel.StreamPlatform, StreamPlatformType.Twitch);
            Assert.AreEqual(youTubeChannel.ChannelName, "Youtube channel");
            Assert.AreEqual(youTubeChannel.StreamPlatform, StreamPlatformType.YouTube);
        }

        [Test]
        public async Task Should_Update_The_Channel_If_The_Channel_Already_Exists_For_The_Platform()
        {
            dataStore.Clear();

            dataStore.Add(new Channel
            {
                ChannelName = "Youtube channel",
                StreamPlatform = StreamPlatformType.YouTube,
                AvatarUrl = "old avatar url",
                ChannelUrl = "old channel url",
            });

            channelRepositoryStub.Setup(s => s.Get(StreamPlatformType.YouTube, "Youtube channel")).ReturnsAsync(() => Maybe<Channel>.Just(dataStore[0]));
            channelRepositoryStub.Setup(s => s.Update(It.IsAny<Channel>())).Callback<Channel>(channel => dataStore[0] = channel);

            StreamerChannelDto updatedChannelDto = new StreamerChannelDto
            {
                ChannelName = "Youtube channel",
                Platform = StreamPlatformType.YouTube,
                AvatarUrl = "new avatar url",
                ChannelUrl = "new channel url",
            };

            youTubeStreamProviderStub
                .Setup(s => s.GetStreamerChannel(updatedChannelDto.ChannelName))
                .ReturnsAsync(MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Success(updatedChannelDto));

            await channelController.AddChannel(StreamPlatformType.YouTube, "Youtube channel");

            var updateResponse = await channelController.AddChannel(StreamPlatformType.YouTube, "Youtube channel");

            var result = updateResponse as OkObjectResult;

            var getResponse = await channelController.GetChannel(StreamPlatformType.YouTube, "Youtube channel");
            var getValue = (getResponse as OkObjectResult).Value;
            var channel = getValue as ChannelDto;

            Assert.AreEqual(dataStore.Count(), 1);
            Assert.IsInstanceOf<NoContentResult>(updateResponse);
            Assert.AreEqual(channel.AvatarUrl, updatedChannelDto.AvatarUrl);
            Assert.AreEqual(channel.ChannelUrl, updatedChannelDto.ChannelUrl);
        }

        [Test]
        public async Task Should_Respond_With_Bad_Request_Error_When_Channel_Does_Not_Exist_On_The_Platform()
        {
            twitchtreamProviderStub
                .Setup(s => s.GetStreamerChannel("Fake Streamer"))
                .ReturnsAsync(MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Success(Maybe<StreamerChannelDto>.Nothing()));

            var response = await channelController.AddChannel(StreamPlatformType.Twitch, "Fake Streamer");
            var result = response as BadRequestObjectResult;
            var value = result.Value as ErrorResponseContract;

            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            Assert.AreEqual(value.Errors.First().ErrorCode, ErrorCodeType.ChannelNotFoundOnPlatform);
        }


        [Test]
        public async Task Should_Respond_With_Failed_Dependency_If_The_Platform_Service_Is_Not_Available()
        {
            youTubeStreamProviderStub
                .Setup(s => s.GetStreamerChannel("Fake Streamer"))
                .ReturnsAsync(MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Fail(GetStreamerChannelErrorType.ProviderNotAvailable));

            var response = await channelController.AddChannel(StreamPlatformType.YouTube, "Fake Streamer");
            var result = response as ObjectResult;
            var value = result.Value as ErrorResponseContract;

            Assert.AreEqual(result.StatusCode, StatusCodes.Status424FailedDependency);
            Assert.AreEqual(value.Errors.First().ErrorCode, ErrorCodeType.PlatformServiceIsNotAvailable);
        }
    }
}
