using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Api.Controllers;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application.Interactors;
using GameStreamSearch.Application.Providers;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Repositories.InMemoryRepositories;
using GameStreamSearch.StreamProviders;
using GameStreamSearch.StreamProviders.Builders;
using GameStreamSearch.StreamProviders.ProviderApi.Twitch.Dto.Kraken;
using GameStreamSearch.StreamProviders.ProviderApi.Twitch.Interfaces;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Dto.YouTubeV3;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace GameStreamSearch.Api.Tests
{
    public class StreamerTests
    {
        private StreamerController streamerController;
        private IStreamService streamService;
        private IStreamerRepository streamerRepository;
        private IInteractor<StreamerDto, IRegisterStreamerPresenter> registerStreamerInteractor;
        private IInteractor<string, IGetStreamerByIdPresenter> getStreamerByIdInteractor;
        private IStreamProvider twitchStreamProvider;
        private IStreamProvider youTubeStreamProvider;

        private Mock<ITwitchKrakenApi> twitchKrakenApiStub;
        private Mock<IYouTubeV3Api> youTubeApiStub;
        private Mock<ITimeProvider> timeProviderStub;

        private readonly DateTime registrationDate = DateTime.Now;


        [SetUp]
        public void Setup()
        {
            Mock<IPaginator> paginator = new Mock<IPaginator>();

            twitchKrakenApiStub = new Mock<ITwitchKrakenApi>();
            youTubeApiStub = new Mock<IYouTubeV3Api>();

            twitchStreamProvider = new TwitchStreamProvider(StreamingPlatform.twitch.GetFriendlyName(), twitchKrakenApiStub.Object);
            youTubeStreamProvider = new YouTubeStreamProvider(StreamingPlatform.dlive.GetFriendlyName(), new YouTubeWatchUrlBuilder(""), youTubeApiStub.Object);


            streamService = new StreamService(paginator.Object)
                .RegisterStreamProvider(StreamingPlatform.twitch, twitchStreamProvider)
                .RegisterStreamProvider(StreamingPlatform.youtube, youTubeStreamProvider);
              

            streamerRepository = new StreamerRepository();
            registerStreamerInteractor = new RegisterStreamerInteractor(streamerRepository, streamService);
            getStreamerByIdInteractor = new GetStreamerByIdInteractor(streamerRepository);

            timeProviderStub = new Mock<ITimeProvider>();

            youTubeApiStub.Setup(s => s.SearchChannelsByUsername(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new YouTubeChannelsDto
                {
                    items = new List<YouTubeChannelDto> { new YouTubeChannelDto() }
                });

            timeProviderStub.Setup(s => s.GetNow()).Returns(registrationDate);


            streamerController = new StreamerController(
                registerStreamerInteractor,
                getStreamerByIdInteractor,
                streamerRepository,
                timeProviderStub.Object,
                new IdProvider());


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
            twitchKrakenApiStub.Setup(s => s.SearchChannels(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new TwitchChannelsDto
                {
                    Channels = new List<TwitchChannelDto> { new TwitchChannelDto() }
                });

            var streamer = new RegisterStreamerDto
            {
                Name = "Test Streamer",
                Platform = StreamingPlatform.twitch,
            };

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
        public async Task Should_Register_The_Same_Streamer_To_Be_Registered_For_Multiple_Platforms()
        {
            twitchKrakenApiStub.Setup(s => s.SearchChannels(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new TwitchChannelsDto
                {
                    Channels = new List<TwitchChannelDto> { new TwitchChannelDto() }
                });

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
            twitchKrakenApiStub.Setup(s => s.SearchChannels(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new TwitchChannelsDto
                {
                    Channels = new List<TwitchChannelDto> { new TwitchChannelDto() }
                });

            var streamer = new RegisterStreamerDto
            {
                Name = "Existing Streamer",
                Platform = StreamingPlatform.twitch,
            };

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
            twitchKrakenApiStub.Setup(s => s.SearchChannels(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new TwitchChannelsDto
                {
                    Channels = new List<TwitchChannelDto>()
                });

            var streamer = new RegisterStreamerDto
            {
                Name = "Fake Streamer",
                Platform = StreamingPlatform.twitch,
            };

            var response = await streamerController.RegisterStreamer(streamer);


            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task Should_Register_The_Streamer_If_The_Stream_Provider_Is_Not_Available()
        {
            twitchKrakenApiStub.Setup(s => s.SearchChannels(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new TwitchChannelsDto());

            var streamer = new RegisterStreamerDto
            {
                Name = "New Streamer",
                Platform = StreamingPlatform.twitch,
            };

            await streamerController.RegisterStreamer(streamer);

            var response = await streamerController.GetStreamers();

            Assert.IsInstanceOf<OkObjectResult>(response);
        }
    }
}
