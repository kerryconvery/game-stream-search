using System.Linq;
using GameStreamSearch.Application.Controllers;
using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GameStreamSearch.Domain;
using GameStreamSearch.Application.Dto;

namespace GameStreamSearch.Application.Tests
{
    public class StreamTests
    {
        private StreamController streamController;
        private Mock<IStreamService> streamServiceStub;

        [SetUp]
        public void Setup()
        {
            streamServiceStub = new Mock<IStreamService>();
            streamController = new StreamController(streamServiceStub.Object);
        }

        [Test]
        public async Task Should_Return_A_List_Of_Unfiltered_Live_Streams()
        {
            streamServiceStub.Setup(s => s.GetStreams(It.Is<StreamFilterOptionsDto>(o => o.GameName == null), 1, null)).ReturnsAsync(new GameStreamsDto());

            var response = await streamController.GetStreams(null, 1);
            var responseResult = response as OkObjectResult;
            var responseValue = (response as OkObjectResult).Value as GameStreamsDto;

            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.IsNotNull(responseValue);
        }

        [Test]
        public async Task Should_Return_A_List_Of_Filtered_Live_Streams()
        {
            var game = "test game";

            streamServiceStub.Setup(s => s.GetStreams(It.Is<StreamFilterOptionsDto>(o => o.GameName == game), 1, null)).ReturnsAsync(new GameStreamsDto());

            var response = await streamController.GetStreams(game, 1);
            var responseResult = response as OkObjectResult;
            var responseValue = (response as OkObjectResult).Value as GameStreamsDto;

            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.IsNotNull(responseValue);
        }

        [Test]
        public async Task Should_Return_The_Next_Page_Of_Live_Streams()
        {
            var pageToken = "page token";

            streamServiceStub.Setup(s => s.GetStreams(It.Is<StreamFilterOptionsDto>(o => o.GameName == null), 1, pageToken)).ReturnsAsync(new GameStreamsDto());

            var response = await streamController.GetStreams(null, 1, pageToken);
            var responseResult = response as OkObjectResult;
            var responseValue = (response as OkObjectResult).Value as GameStreamsDto;

            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.IsNotNull(responseValue);
        }
    }
}