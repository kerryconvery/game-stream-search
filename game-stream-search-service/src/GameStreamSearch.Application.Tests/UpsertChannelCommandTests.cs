using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Commands;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;
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

            var upsertChannelCommand = new UpsertChannelCommand(channelRepositoryMock.Object, channelServiceStub.Object);

            var result = await upsertChannelCommand.Invoke(upsertChannelRequest);

            Assert.AreEqual(result, UpsertChannelResult.ChannelAdded);
        }

        [Test]
        public async Task Should_Update_An_Existing_Channel_In_The_Repository()
        {
            var upsertChannelRequest = CreateUpsertRequest("existing channel");

            SetChannelServiceToReturnChannel(upsertChannelRequest.ChannelName);
            SetChannelRepositoryToReturnAChannel(upsertChannelRequest.ChannelName);

            var upsertChannelCommand = new UpsertChannelCommand(channelRepositoryMock.Object, channelServiceStub.Object);

            var result = await upsertChannelCommand.Invoke(upsertChannelRequest);

            Assert.AreEqual(result, UpsertChannelResult.ChannelUpdated);
        }

        [Test]
        public async Task Should_Return_Channel_Not_Found_If_The_Channel_Does_Not_Exist_On_The_Streaming_Platform()
        {
            var upsertChannelRequest = CreateUpsertRequest("new channel");

            SetChannelServiceToReturnNothing();
            SetChannelRepositoryToReturnAChannel(upsertChannelRequest.ChannelName);

            var upsertChannelCommand = new UpsertChannelCommand(channelRepositoryMock.Object, channelServiceStub.Object);

            var result = await upsertChannelCommand.Invoke(upsertChannelRequest);

            Assert.AreEqual(result, UpsertChannelResult.ChannelNotFoundOnPlatform);
        }

        [Test]
        public async Task Should_Return_Platform_Not_Available_If_Channel_Service_Returns_An_Error()
        {
            var upsertChannelRequest = CreateUpsertRequest("new channel");

            SetChannelServiceToReturnAnError();
            SetChannelRepositoryToReturnAChannel(upsertChannelRequest.ChannelName);

            var upsertChannelCommand = new UpsertChannelCommand(channelRepositoryMock.Object, channelServiceStub.Object);

            var result = await upsertChannelCommand.Invoke(upsertChannelRequest);

            Assert.AreEqual(result, UpsertChannelResult.PlatformServiceIsNotAvailable);
        }

        private UpsertChannelRequest CreateUpsertRequest(string channelName)
        {
            return new UpsertChannelRequest
            {
                ChannelName = channelName,
                RegistrationDate = DateTime.Now,
                StreamPlatform = StreamPlatformType.YouTube
            };
        }

        private void SetChannelServiceToReturnNothing()
        {
            channelServiceStub.Setup(s => s.GetStreamerChannel(It.IsAny<string>(), It.IsAny<StreamPlatformType>()))
                .ReturnsAsync(MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Success(Maybe<StreamerChannelDto>.Nothing));
        }

        private void SetChannelServiceToReturnChannel(string channelName)
        {
            var channel = new StreamerChannelDto
            {
                ChannelName = channelName,
                Platform = StreamPlatformType.YouTube,
                AvatarUrl = "https://avatar.url",
                ChannelUrl = "https://channel.url",
            };

            channelServiceStub.Setup(s => s.GetStreamerChannel(It.IsAny<string>(), It.IsAny<StreamPlatformType>()))
                .ReturnsAsync(MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Success(Maybe<StreamerChannelDto>.Some(channel)));
        }

        private void SetChannelServiceToReturnAnError()
        {
            channelServiceStub.Setup(s => s.GetStreamerChannel(It.IsAny<string>(), It.IsAny<StreamPlatformType>()))
                .ReturnsAsync(MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Fail(GetStreamerChannelErrorType.ProviderNotAvailable));
        }

        private void SetChannelRepositoryToReturnNothing()
        {
            channelRepositoryMock.Setup(m => m.Get(It.IsAny<StreamPlatformType>(), It.IsAny<string>()))
                .ReturnsAsync(Maybe<Channel>.Nothing);
        }

        private void SetChannelRepositoryToReturnAChannel(string channelName)
        {
            var existingChannel = new Channel
            {
                ChannelName = channelName,
                DateRegistered = DateTime.Now,
                StreamPlatform = StreamPlatformType.YouTube,
                AvatarUrl = "https://avatar.url",
                ChannelUrl = "https://channel.url",
            };

            channelRepositoryMock.Setup(m => m.Get(It.IsAny<StreamPlatformType>(), It.IsAny<string>()))
                .ReturnsAsync(Maybe<Channel>.Some(existingChannel));

        }
    }
}
