using System;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Api.Controllers;
using GameStreamSearch.Api.Dto;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace GameStreamSearch.Api.Tests
{
    public class StreamerTests
    {

        private StreamerController streamerController;


        [SetUp]
        public void Setup()
        {
            streamerController = new StreamerController(null);
        }

        [Test]
        public async Task Should_Register_A_New_Streamer()
        {
            var streamer = new RegisterStreamerDto
            {
                Name = "Test Streamer",
                Platform = "twitch",
            };

            await streamerController.RegisterStreamer(streamer);

            var streamers = await streamerController.GetStreamers();


            Assert.AreEqual(streamers.Value.Count(), 1);
            Assert.AreEqual(streamers.Value.First().Name, streamer.Name);
            Assert.AreEqual(streamers.Value.First().Platform, streamer.Platform);
        }


        [Test]
        public async Task Should_Respond_With_Bad_Request_When_The_Request_Contains_Invalid_Data()
        {
            var streamer = new RegisterStreamerDto
            {
                Name = "Test Streamer",
            };

            await streamerController.RegisterStreamer(streamer);

            var streamers = await streamerController.GetStreamers();

            Assert.IsInstanceOf<BadRequestResult>(streamers.Result);
        }


        [Test]
        public async Task Should_Respond_With_Bad_Request_When_Streamer_Is_Already_Registered_For_The_Platform()
        {
            var streamer = new RegisterStreamerDto
            {
                Name = "Existing Streamer",
                Platform = "twitch",
            };

            await streamerController.RegisterStreamer(streamer);

            var response = await streamerController.GetStreamers();

            Assert.IsInstanceOf<BadRequestResult>(response);

        }

        [Test]
        public async Task Should_Respond_With_Bad_Request_When_Streamer_Does_Not_Exist_On_The_Platform()
        {
            var streamer = new RegisterStreamerDto
            {
                Name = "Fake Streamer",
                Platform = "twitch",
            };

            await streamerController.RegisterStreamer(streamer);

            var streamers = await streamerController.GetStreamers();

            Assert.IsInstanceOf<BadRequestResult>(streamers.Result);
        }
    }
}
