using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Commands;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Types;
using Moq;
using NUnit.Framework;

namespace GameStreamSearch.Application.Tests
{
    public class UpsertChannelCommandTests
    {
        private Mock<IChannelRepository> channelRepositoryMock = new Mock<IChannelRepository>();
        private Mock<IChannelService> channelServiceStub = new Mock<IChannelService>();

        [Test]
        public async Task Should_Add_A_New_Channel_To_The_Repository()
        {
            var upsertChannelRequest = CreateUpsertRequest("new channel");

            SetChannelServiceToReturnChannel(upsertChannelRequest.ChannelName);
            SetChannelRepositoryToReturnNothing();

            var upsertChannelCommand = new RegisterOrUpdateChannelCommandHandler(channelRepositoryMock.Object, channelServiceStub.Object);

            var result = await upsertChannelCommand.Handle(upsertChannelRequest);

            Assert.AreEqual(result, RegisterOrUpdateChannelCommandResult.ChannelAdded);
        }

        [Test]
        public async Task Should_Update_An_Existing_Channel_In_The_Repository()
        {
            var upsertChannelRequest = CreateUpsertRequest("existing channel");

            SetChannelServiceToReturnChannel(upsertChannelRequest.ChannelName);
            SetChannelRepositoryToReturnAChannel(upsertChannelRequest.ChannelName);

            var upsertChannelCommand = new RegisterOrUpdateChannelCommandHandler(channelRepositoryMock.Object, channelServiceStub.Object);

            var result = await upsertChannelCommand.Handle(upsertChannelRequest);

            Assert.AreEqual(result, RegisterOrUpdateChannelCommandResult.ChannelUpdated);
        }

        [Test]
        public async Task Should_Return_Channel_Not_Found_If_The_Channel_Does_Not_Exist_On_The_Streaming_Platform()
        {
            var upsertChannelRequest = CreateUpsertRequest("new channel");

            SetChannelServiceToReturnNothing();
            SetChannelRepositoryToReturnAChannel(upsertChannelRequest.ChannelName);

            var upsertChannelCommand = new RegisterOrUpdateChannelCommandHandler(channelRepositoryMock.Object, channelServiceStub.Object);

            var result = await upsertChannelCommand.Handle(upsertChannelRequest);

            Assert.AreEqual(result, RegisterOrUpdateChannelCommandResult.ChannelNotFoundOnPlatform);
        }

        [Test]
        public async Task Should_Return_Platform_Not_Available_If_Channel_Service_Returns_An_Error()
        {
            var upsertChannelRequest = CreateUpsertRequest("new channel");

            SetChannelServiceToReturnAnError();
            SetChannelRepositoryToReturnAChannel(upsertChannelRequest.ChannelName);

            var upsertChannelCommand = new RegisterOrUpdateChannelCommandHandler(channelRepositoryMock.Object, channelServiceStub.Object);

            var result = await upsertChannelCommand.Handle(upsertChannelRequest);

            Assert.AreEqual(result, RegisterOrUpdateChannelCommandResult.PlatformServiceIsNotAvailable);
        }

        private RegisterOrUpdateChannelCommand CreateUpsertRequest(string channelName)
        {
            return new RegisterOrUpdateChannelCommand
            {
                ChannelName = channelName,
                RegistrationDate = DateTime.Now,
                StreamPlatformId = "youtube", 
            };
        }

        private void SetChannelServiceToReturnNothing()
        {
            channelServiceStub.Setup(s => s.GetStreamerChannel(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(MaybeResult<PlatformChannelDto, StreamProviderError>.Success(Maybe<PlatformChannelDto>.Nothing));
        }

        private void SetChannelServiceToReturnChannel(string channelName)
        {
            var channel = new PlatformChannelDto
            {
                ChannelName = channelName,
                StreamPlatformId = "youtube",
                AvatarUrl = "https://avatar.url",
                ChannelUrl = "https://channel.url",
            };

            channelServiceStub.Setup(s => s.GetStreamerChannel(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(MaybeResult<PlatformChannelDto, StreamProviderError>.Success(Maybe<PlatformChannelDto>.Some(channel)));
        }

        private void SetChannelServiceToReturnAnError()
        {
            channelServiceStub.Setup(s => s.GetStreamerChannel(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(MaybeResult<PlatformChannelDto, StreamProviderError>.Fail(StreamProviderError.ProviderNotAvailable));
        }

        private void SetChannelRepositoryToReturnNothing()
        {
            channelRepositoryMock.Setup(m => m.Get(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(Maybe<Channel>.Nothing);
        }

        private void SetChannelRepositoryToReturnAChannel(string channelName)
        {
            var existingChannel = new Channel(channelName, "youtube", DateTime.Now, "https://avatar.url", "https://channel.url");

            channelRepositoryMock.Setup(m => m.Get(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(Maybe<Channel>.Some(existingChannel));

        }
    }
}
