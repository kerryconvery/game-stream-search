using System;
using NUnit.Framework;

namespace GameStreamSearch.Types.Tests
{
    public enum TestErrorType
    {
        None,
        Error,
    }

    public class ResultTests
    {
        [Test]
        public void Should_Be_Success()
        {
            var successResult = Result<TestErrorType>.Success();

            Assert.IsTrue(successResult.IsSuccess);
            Assert.IsFalse(successResult.IsFailure);
            Assert.AreEqual(successResult.Error, TestErrorType.None);
        }

        [Test]
        public void Should_Be_Fail()
        {
            var successResult = Result<TestErrorType>.Fail(TestErrorType.Error);

            Assert.IsTrue(successResult.IsFailure);
            Assert.IsFalse(successResult.IsSuccess);
            Assert.AreEqual(successResult.Error, TestErrorType.Error);
        }

        [Test]
        public void Should_Be_Success_With_A_Just_Value()
        {
            var successResult = MaybeResult<int, TestErrorType>.Success(1);

            Assert.IsTrue(successResult.IsSuccess);
            Assert.IsTrue(successResult.Value.IsSome);
            Assert.IsFalse(successResult.IsFailure);
            Assert.AreEqual(successResult.Error, TestErrorType.None);
        }

        [Test]
        public void Should_Be_Fail_With_A_Nothing_Value()
        {
            var successResult = MaybeResult<int, TestErrorType>.Fail(TestErrorType.Error);

            Assert.IsTrue(successResult.IsFailure);
            Assert.IsTrue(successResult.Value.IsNothing);
            Assert.AreEqual(successResult.Error, TestErrorType.Error);
            Assert.IsFalse(successResult.IsSuccess);
        }

        [Test]
        public void Should_Be_Success_With_A_Maybe_Value()
        {
            var maybeValue = Maybe<int>.Some(1);
            var successResult = MaybeResult<int, TestErrorType>.Success(maybeValue);

            Assert.IsTrue(successResult.IsSuccess);
            Assert.AreEqual(successResult.Value, maybeValue);
            Assert.IsFalse(successResult.IsFailure);
            Assert.AreEqual(successResult.Error, TestErrorType.None);
        }
    }
}
