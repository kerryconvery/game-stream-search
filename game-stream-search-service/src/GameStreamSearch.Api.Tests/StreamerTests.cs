using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application.Controllers;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Domain;
using GameStreamSearch.Domain.Dto;
using GameStreamSearch.Domain.Enums;
using GameStreamSearch.Domain.Exceptions;
using GameStreamSearch.Domain.Interactors;
using GameStreamSearch.Domain.Providers;
using GameStreamSearch.Repositories.InMemoryRepositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace GameStreamSearch.Application.Tests
{
    public class StreamerTests
    {
        private ChannelController streamerController;
        private Mock<IStreamService> streamServiceStub;
        private IChannelRepository streamerRepository;
        private IInteractor<ChannelDto, IRegisterChannelPresenter> RegisterChannelInteractor;
        private IInteractor<string, IGetStreamerByIdPresenter> getStreamerByIdInteractor;
        private Mock<ITimeProvider> timeProviderStub;

        private readonly DateTime registrationDate = DateTime.Now;


        [SetUp]
        public void Setup()
        {
            streamServiceStub = new Mock<IStreamService>();

            streamerRepository = new InMemoryChannelRepository();
            RegisterChannelInteractor = new RegisterChannelInteractor(streamerRepository, streamServiceStub.Object);
            getStreamerByIdInteractor = new GetStreamerByIdInteractor(streamerRepository);

            timeProviderStub = new Mock<ITimeProvider>();

            timeProviderStub.Setup(s => s.GetNow()).Returns(registrationDate);


            streamerController = new ChannelController(
                RegisterChannelInteractor,
                getStreamerByIdInteractor,
                streamerRepository,
                timeProviderStub.Object,
                new GuidIdProvider());


            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();

            urlHelper.Setup(s => s.Link(nameof(streamerController.GetStreamerById), It.IsAny<object>())).Returns<string, GetStreamerByIdParams>((routeName, routeParams) =>
            {
                return routeParams.Id;
            });

            streamerController.Url = urlHelper.Object;
        }

        [Test]
        public async Task Should_Register_A_New_Streamer()
        {
            var channel = new RegisterChannelDto
            {
                ChannelName = "Test Channel",
                Platform = StreamPlatformType.twitch,
            };

            streamServiceStub.Setup(s => s.GetChannel(channel.ChannelName, channel.Platform)).ReturnsAsync(new ChannelDto());

            var createResponse = await streamerController.RegisterChannel(channel);
            var createResult = createResponse as CreatedResult;

            var response = await streamerController.GetStreamers();

            var okResult = response as OkObjectResult;
            var value = okResult.Value as IEnumerable<ChannelDto>;

            Assert.AreEqual(value.Count(), 1);
            Assert.IsNotNull(value.First().Id);
            Assert.AreEqual(value.First().Name, channel.ChannelName);
            Assert.AreEqual(value.First().StreamPlatform, channel.Platform);
            Assert.AreEqual(value.First().DateRegistered, registrationDate);
            Assert.AreEqual(createResult.Location, value.First().Id);
        }

        [Test]
        public async Task Should_Allow_Registering_The_Same_Streamer_For_Multiple_Platforms()
        {
            var twitchStreamer = new RegisterChannelDto
            {
                ChannelName = "Test Channel",
                Platform = StreamPlatformType.twitch,
            };

            var youtubeStreamer = new RegisterChannelDto
            {
                ChannelName = "Test Channel",
                Platform = StreamPlatformType.youtube,
            };

            streamServiceStub.Setup(s => s.GetChannel(twitchStreamer.ChannelName, twitchStreamer.Platform)).ReturnsAsync(new ChannelDto());
            streamServiceStub.Setup(s => s.GetChannel(youtubeStreamer.ChannelName, youtubeStreamer.Platform)).ReturnsAsync(new ChannelDto());

            await streamerController.RegisterChannel(twitchStreamer);
            await streamerController.RegisterChannel(youtubeStreamer);

            var response = await streamerController.GetStreamers();

            var okAction = response as OkObjectResult;
            var value = okAction.Value as IEnumerable<ChannelDto>;

            Assert.AreEqual(value.First().Name, twitchStreamer.ChannelName);
            Assert.AreEqual(value.First().StreamPlatform, twitchStreamer.Platform);
            Assert.AreEqual(value.Last().Name, youtubeStreamer.ChannelName);
            Assert.AreEqual(value.Last().StreamPlatform, youtubeStreamer.Platform);
        }

        [Test]
        public async Task Should_Respond_With_Created_If_The_Streamer_Is_Already_Registered_For_The_Platform()
        {
            var channel = new RegisterChannelDto
            {
                ChannelName = "Existing Channel",
                Platform = StreamPlatformType.twitch,
            };

            streamServiceStub.Setup(s => s.GetChannel(channel.ChannelName, channel.Platform)).ReturnsAsync(new ChannelDto());

            var createResponseNew = await streamerController.RegisterChannel(channel);
            var createNewResult = createResponseNew as CreatedResult;

            var createResponseExisting = await streamerController.RegisterChannel(channel);
            var createExistingResult = createResponseExisting as CreatedResult;

            var response = await streamerController.GetStreamers();
            var okResult = response as OkObjectResult;
            var value = okResult.Value as IEnumerable<ChannelDto>;

            Assert.AreEqual(value.Count(), 1);
            Assert.AreEqual(createNewResult.Location, createExistingResult.Location);
        }

        [Test]
        public async Task Should_Respond_With_Bad_Request_When_Streamer_Does_Not_Exist_On_The_Platform()
        {
            var channel = new RegisterChannelDto
            {
                ChannelName = "Fake Channel",
                Platform = StreamPlatformType.twitch,
            };

            streamServiceStub.Setup(s => s.GetChannel(channel.ChannelName, channel.Platform)).Returns(Task.FromResult<ChannelDto>(null));

            var response = await streamerController.RegisterChannel(channel);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task Should_Register_The_Streamer_If_The_Stream_Provider_Is_Not_Available()
        {
            var channel = new RegisterChannelDto
            {
                ChannelName = "New Channel",
                Platform = StreamPlatformType.twitch,
            };

            streamServiceStub.Setup(s => s.GetChannel(channel.ChannelName, channel.Platform)).ThrowsAsync(new StreamProviderUnavailableException());

            await streamerController.RegisterChannel(channel);

            var response = await streamerController.GetStreamers();

            Assert.IsInstanceOf<OkObjectResult>(response);
        }
    }
}
