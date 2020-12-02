using System;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Api.Contracts;
using GameStreamSearch.Api.Controllers;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Interactors;
using GameStreamSearch.Application.Providers;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Repositories.InMemoryRepositories;
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

        private readonly DateTime registrationDate = DateTime.Now;

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
            youTubeStreamProviderStub = new Mock<IStreamProvider>();
            youTubeStreamProviderStub.SetupGet(s => s.Platform).Returns(StreamPlatformType.YouTube);
            youTubeStreamProviderStub.Setup(s => s.GetStreamerChannel(youtubeChannelDto.ChannelName)).ReturnsAsync(youtubeChannelDto);

            twitchtreamProviderStub = new Mock<IStreamProvider>();
            twitchtreamProviderStub.SetupGet(s => s.Platform).Returns(StreamPlatformType.Twitch);
            twitchtreamProviderStub.Setup(s => s.GetStreamerChannel(twitchChannelDto.ChannelName)).ReturnsAsync(twitchChannelDto);

            StreamService streamService = new StreamService()
                .RegisterStreamProvider(youTubeStreamProviderStub.Object)
                .RegisterStreamProvider(twitchtreamProviderStub.Object);

            IChannelRepository channelRepository = new InMemoryChannelRepository();

            UpsertChannelInteractor addChannelInteractor = new UpsertChannelInteractor(channelRepository, streamService);
            GetChannelInteractor getChannelInteractor = new GetChannelInteractor(channelRepository);

            timeProviderStub = new Mock<ITimeProvider>();

            timeProviderStub.Setup(s => s.GetNow()).Returns(registrationDate);

            channelController = new ChannelsController(
                addChannelInteractor,
                getChannelInteractor,
                channelRepository,
                timeProviderStub.Object);

            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();

            urlHelper.Setup(s => s.Link(nameof(channelController.GetChannel), It.IsAny<object>())).Returns<string, GetChannelParams>((routeName, routeParams) =>
            {
                return routeParams.Channel;
            });

            channelController.Url = urlHelper.Object;
        }

        [Test]
        public async Task Should_Add_A_New_Channel()
        {
            var createResponse = await channelController.AddChannel(StreamPlatformType.YouTube, "Youtube channel");
            var createResult = createResponse as CreatedResult;
            var value = createResult.Value as ChannelDto;


            Assert.IsInstanceOf<CreatedResult>(createResponse);
            Assert.AreEqual(createResult.Location, value.ChannelName);

            Assert.AreEqual(value.ChannelName, youtubeChannelDto.ChannelName);
            Assert.AreEqual(value.StreamPlatform, youtubeChannelDto.Platform);
            Assert.AreEqual(value.AvatarUrl, youtubeChannelDto.AvatarUrl);
            Assert.AreEqual(value.ChannelUrl, youtubeChannelDto.ChannelUrl);
        }

        [Test]
        public async Task Should_Allow_Adding_The_Same_Channel_For_Multiple_Platforms()
        {
            await channelController.AddChannel(StreamPlatformType.Twitch, "Twitch channel");
            await channelController.AddChannel(StreamPlatformType.YouTube, "Youtube channel");

            var response = await channelController.GetChannels();

            var okAction = response as OkObjectResult;
            var value = okAction.Value as ChannelListDto;

            Assert.AreEqual(value.Items.First().ChannelName, "Twitch channel");
            Assert.AreEqual(value.Items.First().StreamPlatform, StreamPlatformType.Twitch);
            Assert.AreEqual(value.Items.Last().ChannelName, "Youtube channel");
            Assert.AreEqual(value.Items.Last().StreamPlatform, StreamPlatformType.YouTube);
        }

        [Test]
        public async Task Should_Update_The_Channel_And_Respond_With_The_Channel_If_The_Channel_Is_Already_Exists_For_The_Platform()
        {
            StreamerChannelDto updatedChannelDto = new StreamerChannelDto
            {
                ChannelName = "Youtube channel",
                Platform = StreamPlatformType.YouTube,
                AvatarUrl = "new avatar url",
                ChannelUrl = "new channel url",
            };

            youTubeStreamProviderStub.Setup(s => s.GetStreamerChannel(updatedChannelDto.ChannelName)).ReturnsAsync(updatedChannelDto);

            await channelController.AddChannel(StreamPlatformType.YouTube, "Youtube channel");

            var updateResponse = await channelController.AddChannel(StreamPlatformType.YouTube, "Youtube channel");

            var result = updateResponse as OkObjectResult;
            var value = result.Value as ChannelDto;


            var getResponse = await channelController.GetChannels();
            var getResult = getResponse as OkObjectResult;
            var getValue = getResult.Value as ChannelListDto;

            Assert.AreEqual(getValue.Items.Count(), 1);
            Assert.IsInstanceOf<OkObjectResult>(updateResponse);
            Assert.AreEqual(value.AvatarUrl, updatedChannelDto.AvatarUrl);
            Assert.AreEqual(value.ChannelUrl, updatedChannelDto.ChannelUrl);
        }

        [Test]
        public async Task Should_Respond_With_Bad_Request_With_Error_When_Channel_Does_Not_Exist_On_The_Platform()
        {
            var response = await channelController.AddChannel(StreamPlatformType.Twitch, "Fake Streamer");
            var result = response as BadRequestObjectResult;
            var value = result.Value as ErrorResponseContract;

            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            Assert.AreEqual(value.Errors.First().ErrorCode, ErrorCodeType.ChannelNotFoundOnPlatform);
        }
    }
}
