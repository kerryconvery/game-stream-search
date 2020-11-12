using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using Moq;
using NUnit.Framework;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.Tests
{
    public class StreaServiceTests
    {
        [Test]
        public async Task Should_Return_An_Aggregated_List_Of_Streams_From_All_Registered_Providers()
        {
            var paginatorStub = new Mock<IPaginator>();
            var streamProviderStubA = new Mock<IStreamProvider>();
            var streamProviderStubB = new Mock<IStreamProvider>();
            var streamFilterOptions = new StreamFilterOptionsDto();

            paginatorStub.Setup(m => m.decode(It.IsAny<string>())).Returns(new Dictionary<string, string>());

            streamProviderStubA.Setup(m => m.GetLiveStreams(streamFilterOptions, 2, null))
                .ReturnsAsync(new GameStreamsDto() { Items = new List<GameStreamDto>() { new GameStreamDto() } } );
            streamProviderStubA.SetupGet(m => m.ProviderName).Returns("streamA");

            streamProviderStubB.Setup(m => m.GetLiveStreams(streamFilterOptions, 2, null))
                .ReturnsAsync(new GameStreamsDto() { Items = new List<GameStreamDto>() { new GameStreamDto() } } );
            streamProviderStubB.SetupGet(m => m.ProviderName).Returns("streamB");

            var streamService = new StreamService(paginatorStub.Object)
                .RegisterStreamProvider(StreamingPlatform.twitch, streamProviderStubA.Object)
                .RegisterStreamProvider(StreamingPlatform.youtube, streamProviderStubB.Object);

            var streams = await streamService.GetStreams(streamFilterOptions, 2, null);

            Assert.AreEqual(streams.Items.Count(), 2);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_When_No_Streams_Were_Found()
        {
            var paginatorStub = new Mock<IPaginator>();
            var streamProviderStubA = new Mock<IStreamProvider>();
            var streamFilterOptions = new StreamFilterOptionsDto();

            paginatorStub.Setup(m => m.decode(It.IsAny<string>())).Returns(new Dictionary<string, string>());

            streamProviderStubA.Setup(m => m.GetLiveStreams(streamFilterOptions, 1, null))
                .ReturnsAsync(GameStreamsDto.Empty());
            streamProviderStubA.SetupGet(m => m.ProviderName).Returns("streamA");

            var streamService = new StreamService(paginatorStub.Object)
                .RegisterStreamProvider(StreamingPlatform.twitch, streamProviderStubA.Object);

            var streams = await streamService.GetStreams(streamFilterOptions, 1, null);

            Assert.AreEqual(streams.Items.Count(), 0);
        }


        [Test]
        public async Task Should_Return_An_Aggregated_Next_Page_Tokens()
        {
            var paginatorStub = new Mock<IPaginator>();
            var streamProviderStubA = new Mock<IStreamProvider>();
            var streamProviderStubB = new Mock<IStreamProvider>();
            var streamFilterOptions = new StreamFilterOptionsDto();

            paginatorStub.Setup(m => m.decode(It.IsAny<string>())).Returns(new Dictionary<string, string>());

            streamProviderStubA.Setup(m => m.GetLiveStreams(streamFilterOptions, 1, null))
                .ReturnsAsync(new GameStreamsDto() { Items = new List<GameStreamDto>() { new GameStreamDto() }, NextPageToken = "1" });
            streamProviderStubA.SetupGet(m => m.ProviderName).Returns("streamA");

            streamProviderStubB.Setup(m => m.GetLiveStreams(streamFilterOptions, 1, null))
                .ReturnsAsync(new GameStreamsDto() { Items = new List<GameStreamDto>() { new GameStreamDto() }, NextPageToken = "2" });
            streamProviderStubB.SetupGet(m => m.ProviderName).Returns("streamB");

            paginatorStub.Setup(m => m.encode(It.IsAny<Dictionary<string, string>>())).Returns((Dictionary<string, string> p) =>
            {
                return $"{p[streamProviderStubA.Object.ProviderName]}{p[streamProviderStubB.Object.ProviderName]}";
            });

            var streamService = new StreamService(paginatorStub.Object)
                .RegisterStreamProvider(StreamingPlatform.twitch, streamProviderStubA.Object)
                .RegisterStreamProvider(StreamingPlatform.youtube, streamProviderStubB.Object);

            var streams = await streamService.GetStreams(streamFilterOptions, 1, null);

            Assert.AreEqual(streams.NextPageToken, "12");
        }

        [Test]
        public async Task Should_Exclude_Null_Page_Tokens_From_Aggregation()
        {
            var paginatorStub = new Mock<IPaginator>();
            var streamProviderStubA = new Mock<IStreamProvider>();
            var streamProviderStubB = new Mock<IStreamProvider>();
            var streamFilterOptions = new StreamFilterOptionsDto();

            paginatorStub.Setup(m => m.decode(It.IsAny<string>())).Returns(new Dictionary<string, string>());

            streamProviderStubA.Setup(m => m.GetLiveStreams(streamFilterOptions, 1, null))
                .ReturnsAsync(new GameStreamsDto() { Items = new List<GameStreamDto>() { new GameStreamDto() }, NextPageToken = null });
            streamProviderStubA.SetupGet(m => m.ProviderName).Returns("streamA");

            streamProviderStubB.Setup(m => m.GetLiveStreams(streamFilterOptions, 1, null))
                .ReturnsAsync(new GameStreamsDto() { Items = new List<GameStreamDto>() { new GameStreamDto() }, NextPageToken = "2" });
            streamProviderStubB.SetupGet(m => m.ProviderName).Returns("streamB");

            paginatorStub.Setup(m => m.encode(It.Is<Dictionary<string, string>>(p => !p.ContainsKey("streamA")))).Returns((Dictionary<string, string> p) =>
            {
                return p[streamProviderStubB.Object.ProviderName];
            });

            var streamService = new StreamService(paginatorStub.Object)
                .RegisterStreamProvider(StreamingPlatform.twitch, streamProviderStubA.Object)
                .RegisterStreamProvider(StreamingPlatform.youtube, streamProviderStubB.Object);

            var streams = await streamService.GetStreams(streamFilterOptions, 1, null);

            Assert.AreEqual(streams.NextPageToken, "2");
        }

        [Test]
        public async Task Should_Pass_Page_Tokens_Where_A_Page_Token_For_The_Provider_Exists()
        {
            var paginatorStub = new Mock<IPaginator>();
            var streamProviderStubA = new Mock<IStreamProvider>();
            var streamProviderStubB = new Mock<IStreamProvider>();
            var streamFilterOptions = new StreamFilterOptionsDto();

            var pageTokens = new Dictionary<string, string>
            {
                { "streamB", "streamB.token" }
            };

            paginatorStub.Setup(m => m.decode("encoded.composit.token")).Returns(pageTokens);

            streamProviderStubA.Setup(m => m.GetLiveStreams(streamFilterOptions, 2, null))
                .ReturnsAsync(new GameStreamsDto() { Items = new List<GameStreamDto>() { new GameStreamDto() } });
            streamProviderStubA.SetupGet(m => m.ProviderName).Returns("streamA");

            streamProviderStubB.Setup(m => m.GetLiveStreams(streamFilterOptions, 2, "streamB.token"))
                .ReturnsAsync(new GameStreamsDto() { Items = new List<GameStreamDto>() { new GameStreamDto() } });
            streamProviderStubB.SetupGet(m => m.ProviderName).Returns("streamB");

            var streamService = new StreamService(paginatorStub.Object)
                .RegisterStreamProvider(StreamingPlatform.twitch, streamProviderStubA.Object)
                .RegisterStreamProvider(StreamingPlatform.youtube, streamProviderStubB.Object);

            var streams = await streamService.GetStreams(streamFilterOptions, 2, "encoded.composit.token");

            Assert.AreEqual(streams.Items.Count(), 2);
        }

        [Test]
        public async Task Should_Return_A_Stream_List_Sorted_By_Views()
        {
            var paginatorStub = new Mock<IPaginator>();
            var streamProviderStubA = new Mock<IStreamProvider>();
            var streamFilterOptions = new StreamFilterOptionsDto();

            paginatorStub.Setup(m => m.decode(It.IsAny<string>())).Returns(new Dictionary<string, string>());

            streamProviderStubA.Setup(m => m.GetLiveStreams(streamFilterOptions, 2, null))
                .ReturnsAsync(new GameStreamsDto() { Items = new List<GameStreamDto>() {
                    new GameStreamDto { Views = 1 },
                    new GameStreamDto { Views = 2 }
                } });
            streamProviderStubA.SetupGet(m => m.ProviderName).Returns("streamA");

            var streamService = new StreamService(paginatorStub.Object)
                .RegisterStreamProvider(StreamingPlatform.twitch, streamProviderStubA.Object);

            var streams = await streamService.GetStreams(streamFilterOptions, 2, null);

            Assert.AreEqual(streams.Items.First().Views, 2);
            Assert.AreEqual(streams.Items.Last().Views, 1);
        }
    }
}