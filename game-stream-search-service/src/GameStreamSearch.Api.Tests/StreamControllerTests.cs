using System.Linq;
using GameStreamSearch.Api.Controllers;
using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Application.Enums;
using System.Collections.Generic;

namespace GameStreamSearch.Api.Tests
{
    public class StreamControllerTests
    {
        private StreamsController streamController;
        private Mock<IStreamProvider> youTubeStreamProviderStub;

        private readonly GameStreamsDto unfiltereGameStreeams = new GameStreamsDto
        {
            Items = new List<GameStreamDto>()
            {
                new GameStreamDto
                {
                    StreamerName = "stream 1",
                }
            },
            NextPageToken = "page two"
        };

        private readonly GameStreamsDto filtereGameStreeams = new GameStreamsDto
        {
            Items = new List<GameStreamDto>()
            {
                new GameStreamDto
                {
                    StreamerName = "stream 2",
                }
            }
    };

        private readonly GameStreamsDto nextPageGameStreeams = new GameStreamsDto
        {
            Items = new List<GameStreamDto>()
            {
                new GameStreamDto
                {
                    StreamerName = "stream 3",
                }
            }
        };

        [SetUp]
        public void Setup()
        {
            youTubeStreamProviderStub = new Mock<IStreamProvider>();
            youTubeStreamProviderStub.SetupGet(s => s.Platform).Returns(StreamPlatformType.YouTube);

            youTubeStreamProviderStub.Setup(s => s.GetLiveStreams(It.Is<StreamFilterOptions>(o => o.GameName == null), 1, string.Empty))
                .ReturnsAsync(unfiltereGameStreeams);

            youTubeStreamProviderStub.Setup(s => s.GetLiveStreams(It.Is<StreamFilterOptions>(o => o.GameName == "stream 2"), 1, string.Empty))
                .ReturnsAsync(filtereGameStreeams);

            youTubeStreamProviderStub.Setup(s => s.GetLiveStreams(It.Is<StreamFilterOptions>(o => o.GameName == null), 1, "page two"))
                .ReturnsAsync(nextPageGameStreeams);

            ProviderAggregationService streamService = new ProviderAggregationService()
                .RegisterStreamProvider(youTubeStreamProviderStub.Object);

            streamController = new StreamsController(streamService);
        }

        [Test]
        public async Task Should_Return_A_List_Of_Unfiltered_Live_Streams()
        {
            var response = await streamController.GetStreams(null, 1);
            var responseValue = (response as OkObjectResult).Value as GameStreamsDto;

            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.AreEqual(responseValue.Items.First().StreamerName, "stream 1");
        }

        [Test]
        public async Task Should_Return_A_List_Of_Filtered_Live_Streams()
        {
            var response = await streamController.GetStreams("stream 2", 1);
            var responseValue = (response as OkObjectResult).Value as GameStreamsDto;

            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.AreEqual(responseValue.Items.First().StreamerName, "stream 2");
        }

        [Test]
        public async Task Should_Return_The_Next_Page_Of_Live_Streams()
        {
            var pageOneResponse = await streamController.GetStreams(null, 1);
            var pageOneValue = (pageOneResponse as OkObjectResult).Value as GameStreamsDto;

            var pageeTwoResponse = await streamController.GetStreams(null, 1, pageOneValue.NextPageToken);
            var pageTwoValue = (pageeTwoResponse as OkObjectResult).Value as GameStreamsDto;

            Assert.AreEqual(pageTwoValue.Items.First().StreamerName, "stream 3");
        }
    }
}