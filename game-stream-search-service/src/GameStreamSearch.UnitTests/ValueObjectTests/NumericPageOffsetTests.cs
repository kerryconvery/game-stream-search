using System;
using GameStreamSearch.Application.ValueObjects;
using NUnit.Framework;

namespace GameStreamSearch.UnitTests.ValueObjects
{
    public class NumericPageOffsetTests
    {
        [Test]
        public void Should_Return_The_Offset_When_The_Offset_String_Is_Non_Zero()
        {
            var pageOffset = new NumericPageOffset(1, "1");

            Assert.AreEqual((string)pageOffset, "1");
        }

        [Test]
        public void Should_Return_Offset_As_String_When_The_String_OffSet_Is_Non_Zero()
        {
            var pageOffset = new NumericPageOffset(1, "1");

            Assert.AreEqual((string)pageOffset, "1");
        }


        [Test]
        public void Should_Return_An_Empty_String_String_When_The_String_Empty()
        {
            var pageOffset = new NumericPageOffset(1, string.Empty);

            Assert.AreEqual((string)pageOffset, string.Empty);
        }

        [Test]
        public void Should_Return_The_Next_Page_Offset_When_The_Page_Count_Is_Equal_To_The_Page_Size()
        {
            var pageOffset = new NumericPageOffset(1, "1");

            var nextPage = pageOffset.GetNextOffset(10);

            Assert.AreEqual((string)nextPage, "2");
        }

        [Test]
        public void Should_Return_An_Empty_String_Offset_When_The_Page_Count_Is_Less_Than_The_Page_Size()
        {
            var pageOffset = new NumericPageOffset(10, "1");

            var nextPage = pageOffset.GetNextOffset(9);

            Assert.AreEqual((string)nextPage, string.Empty);
        }

        [Test]
        public void Should_Return_A_String_Offset_As_An_Int()
        {
            var pageOffset = new NumericPageOffset(1, "2");

            Assert.AreEqual((int)pageOffset, 2);
        }


        [Test]
        public void Should_Return_A_Zero_Offset_When_The_String_Offset_Is_Empty()
        {
            var pageOffset = new NumericPageOffset(10, string.Empty);

            Assert.AreEqual((int)pageOffset, 0);
        }

        [Test]
        public void Should_Return_The_Offset_As_An_Int_When_The_Page_Count_Is_Equal_To_The_Page_Size()
        {
            var pageOffset = new NumericPageOffset(1, "1");

            var nextPage = pageOffset.GetNextOffset(10);

            Assert.AreEqual((int)nextPage, 2);
        }

        [Test]
        public void Should_Return_A_Zero_Offset_When_The_Page_Count_Is_Less_Than_The_Page_Size()
        {
            var pageOffset = new NumericPageOffset(10, "1");

            var nextPage = pageOffset.GetNextOffset(9);

            Assert.AreEqual((int)nextPage, 0);
        }
    }
}
