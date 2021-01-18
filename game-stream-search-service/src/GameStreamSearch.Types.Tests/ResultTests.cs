using System;
using NUnit.Framework;

namespace GameStreamSearch.Types.Tests
{
    public class ResultTests
    {
        [Test]
        public void Should_Be_Success()
        {
            var successResult = Result<string>.Success();

            Assert.IsTrue(successResult.IsSuccess);
            Assert.IsFalse(successResult.IsFailure);
            Assert.IsNull(successResult.Error);
        }

        [Test]
        public void Should_Be_Fail()
        {
            var successResult = Result<string>.Fail("error message");

            Assert.IsTrue(successResult.IsFailure);
            Assert.IsFalse(successResult.IsSuccess);
            Assert.AreEqual(successResult.Error, "error message");
        }

        [Test]
        public void Should_Be_Success_With_A_Value()
        {
            var successResult = Result<int, string>.Success(1);

            Assert.IsTrue(successResult.IsSuccess);
            Assert.AreEqual(successResult.Value, 1);
            Assert.IsFalse(successResult.IsFailure);
            Assert.IsNull(successResult.Error);
        }

        [Test]
        public void Should_Be_Success_With_A_Just_Value()
        {
            var successResult = MaybeResult<int, string>.Success(1);

            Assert.IsTrue(successResult.IsSuccess);
            Assert.IsTrue(successResult.Value.IsJust);
            Assert.IsFalse(successResult.IsFailure);
            Assert.IsNull(successResult.Error);
        }

        [Test]
        public void Should_Be_Fail_With_A_Nothing_Value()
        {
            var successResult = MaybeResult<int, string>.Fail("error message");

            Assert.IsTrue(successResult.IsFailure);
            Assert.IsTrue(successResult.Value.IsNothing);
            Assert.AreEqual(successResult.Error, "error message");
            Assert.IsFalse(successResult.IsSuccess);
        }

        [Test]
        public void Should_Be_Success_With_A_Maybe_Value()
        {
            var maybeValue = Maybe<int>.Just(1);
            var successResult = MaybeResult<int, string>.Success(maybeValue);

            Assert.IsTrue(successResult.IsSuccess);
            Assert.AreEqual(successResult.Value, maybeValue);
            Assert.IsFalse(successResult.IsFailure);
            Assert.IsNull(successResult.Error);
        }
    }
}
