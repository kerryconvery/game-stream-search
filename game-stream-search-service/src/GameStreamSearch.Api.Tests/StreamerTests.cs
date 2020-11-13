using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Api.Controllers;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Exceptions;
using GameStreamSearch.Application.Interactors;
using GameStreamSearch.Application.Providers;
using GameStreamSearch.Repositories.InMemoryRepositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace GameStreamSearch.Api.Tests
{
    public class StreamerTests
    {
        private StreamerController streamerController;
        private Mock<IStreamService> streamServiceStub;
        private IStreamerRepository streamerRepository;
        private IInteractor<StreamerDto, IRegisterStreamerPresenter> registerStreamerInteractor;
        private IInteractor<string, IGetStreamerByIdPresenter> getStreamerByIdInteractor;
        private Mock<ITimeProvider> timeProviderStub;

        private readonly DateTime registrationDate = DateTime.Now;


        [SetUp]
        public void Setup()
        {
            streamServiceStub = new Mock<IStreamService>();

            streamerRepository = new InMemoryStreamerRepository();
            registerStreamerInteractor = new RegisterStreamerInteractor(streamerRepository, streamServiceStub.Object);
            getStreamerByIdInteractor = new GetStreamerByIdInteractor(streamerRepository);

            timeProviderStub = new Mock<ITimeProvider>();

            timeProviderStub.Setup(s => s.GetNow()).Returns(registrationDate);


            streamerController = new StreamerController(
                registerStreamerInteractor,
                getStreamerByIdInteractor,
                streamerRepository,
                timeProviderStub.Object,
                new GuidIdProvider());


            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();

            urlHelper.Setup(s => s.Link(nameof(streamerController.GetStreamer), It.IsAny<object>())).Returns<string, GetStreamerByIdParams>((routeName, routeParams) =>
            {
                return routeParams.Id;
            });

            streamerController.Url = urlHelper.Object;
        }

        [Test]
        public async Task Should_Register_A_New_Streamer()
        {
            var streamer = new RegisterStreamerDto
            {
                Name = "Test Streamer",
                Platform = StreamingPlatform.twitch,
            };

            streamServiceStub.Setup(s => s.GetStreamerChannel(streamer.Name, streamer.Platform)).ReturnsAsync(new StreamerChannelDto());

            var createResponse = await streamerController.RegisterStreamer(streamer);
            var createResult = createResponse as CreatedResult;

            var response = await streamerController.GetStreamers();

            var okResult = response as OkObjectResult;
            var value = okResult.Value as IEnumerable<StreamerDto>;

            Assert.AreEqual(value.Count(), 1);
            Assert.IsNotNull(value.First().Id);
            Assert.AreEqual(value.First().Name, streamer.Name);
            Assert.AreEqual(value.First().Platform, streamer.Platform);
            Assert.AreEqual(value.First().DateRegistered, registrationDate);
            Assert.AreEqual(createResult.Location, value.First().Id);
        }

        [Test]
        public async Task Should_Allow_Registering_The_Same_Streamer_For_Multiple_Platforms()
        {
            var twitchStreamer = new RegisterStreamerDto
            {
                Name = "Test Streamer",
                Platform = StreamingPlatform.twitch,
            };

            var youtubeStreamer = new RegisterStreamerDto
            {
                Name = "Test Streamer",
                Platform = StreamingPlatform.youtube,
            };

            streamServiceStub.Setup(s => s.GetStreamerChannel(twitchStreamer.Name, twitchStreamer.Platform)).ReturnsAsync(new StreamerChannelDto());
            streamServiceStub.Setup(s => s.GetStreamerChannel(youtubeStreamer.Name, youtubeStreamer.Platform)).ReturnsAsync(new StreamerChannelDto());

            await streamerController.RegisterStreamer(twitchStreamer);
            await streamerController.RegisterStreamer(youtubeStreamer);

            var response = await streamerController.GetStreamers();

            var okAction = response as OkObjectResult;
            var value = okAction.Value as IEnumerable<StreamerDto>;

            Assert.AreEqual(value.First().Name, twitchStreamer.Name);
            Assert.AreEqual(value.First().Platform, twitchStreamer.Platform);
            Assert.AreEqual(value.Last().Name, youtubeStreamer.Name);
            Assert.AreEqual(value.Last().Platform, youtubeStreamer.Platform);
        }

        [Test]
        public async Task Should_Respond_With_Created_If_The_Streamer_Is_Already_Registered_For_The_Platform()
        {
            var streamer = new RegisterStreamerDto
            {
                Name = "Existing Streamer",
                Platform = StreamingPlatform.twitch,
            };

            streamServiceStub.Setup(s => s.GetStreamerChannel(streamer.Name, streamer.Platform)).ReturnsAsync(new StreamerChannelDto());

            var createResponseNew = await streamerController.RegisterStreamer(streamer);
            var createNewResult = createResponseNew as CreatedResult;

            var createResponseExisting = await streamerController.RegisterStreamer(streamer);
            var createExistingResult = createResponseExisting as CreatedResult;

            var response = await streamerController.GetStreamers();
            var okResult = response as OkObjectResult;
            var value = okResult.Value as IEnumerable<StreamerDto>;

            Assert.AreEqual(value.Count(), 1);
            Assert.AreEqual(createNewResult.Location, createExistingResult.Location);
        }

        [Test]
        public async Task Should_Respond_With_Bad_Request_When_Streamer_Does_Not_Exist_On_The_Platform()
        {
            var streamer = new RegisterStreamerDto
            {
                Name = "Fake Streamer",
                Platform = StreamingPlatform.twitch,
            };

            streamServiceStub.Setup(s => s.GetStreamerChannel(streamer.Name, streamer.Platform)).Returns(Task.FromResult<StreamerChannelDto>(null));

            var response = await streamerController.RegisterStreamer(streamer);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task Should_Register_The_Streamer_If_The_Stream_Provider_Is_Not_Available()
        {
            var streamer = new RegisterStreamerDto
            {
                Name = "New Streamer",
                Platform = StreamingPlatform.twitch,
            };

            streamServiceStub.Setup(s => s.GetStreamerChannel(streamer.Name, streamer.Platform)).ThrowsAsync(new StreamProviderUnavailableException());

            await streamerController.RegisterStreamer(streamer);

            var response = await streamerController.GetStreamers();

            Assert.IsInstanceOf<OkObjectResult>(response);
        }
    }
}
