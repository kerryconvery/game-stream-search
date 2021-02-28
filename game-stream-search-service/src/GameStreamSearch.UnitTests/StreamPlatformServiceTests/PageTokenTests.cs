using GameStreamSearch.Application.Services.StreamProvider;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.ModelTests
{
    public class PageTokenTests
    {
        [Test]
        public void Should_Return_The_Page_Token_For_The_Platform()
        {
            var pageTokens = new PageTokens()
                .AddToken("youtube", "youtube token")
                .AddToken("twitch", "twitch token");

            var pageToken = pageTokens.GetTokenOrEmpty("twitch");

            Assert.AreEqual(pageToken.Token, "twitch token");
        }

        [Test]
        public void Should_Return_An_Empty_Page_Token_When_One_Was_Not_Found_For_The_Platform()
        {
            var pageTokens = new PageTokens()
                .AddToken("twitch", "twitch token");

            var pageToken = pageTokens.GetTokenOrEmpty("youtube");

            Assert.True(pageToken.IsEmpty());
        }

        [Test]
        public void Should_Pack_And_Unpack_Tokens()
        {
            var pageTokens = new PageTokens()
                .AddToken("youtube", "youtube token")
                .AddToken("twitch", "twitch token");

            var packedTokens = pageTokens.PackTokens();

            var unpackedTokens = PageTokens.UnpackTokens(packedTokens);

            Assert.AreEqual(unpackedTokens.GetTokenOrEmpty("youtube").Token, "youtube token");
            Assert.AreEqual(unpackedTokens.GetTokenOrEmpty("twitch").Token, "twitch token");
        }

        [Test]
        public void Should_Return_An_Empty_String_When_All_Page_Tokens_Are_Empty()
        {
            var pageTokens = new PageTokens()
                .AddToken("youtube", "")
                .AddToken("twitch", "");

            var packedTokens = pageTokens.PackTokens();

            Assert.IsEmpty(packedTokens);
        }

        [Test]
        public void Should_Only_Pack_Non_Empty_Tokens()
        {
            var pageTokens = new PageTokens()
                .AddToken("youtube", "youtube token")
                .AddToken("twitch", "");

            var packedTokens = pageTokens.PackTokens();

            var unpackedTokens = PageTokens.UnpackTokens(packedTokens);

            Assert.AreEqual(unpackedTokens.GetTokenOrEmpty("youtube").Token, "youtube token");
            Assert.AreEqual(unpackedTokens.GetTokenOrEmpty("twitch").Token, "");
        }
    }
}
