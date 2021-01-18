using System;
using NUnit.Framework;

namespace GameStreamSearch.Types.Tests
{
    public class MaybeTests
    {
        [Test]
        public void Should_Return_Be_Just_If_Value_Is_Not_Null()
        {
            var nonNullValue = Maybe<string>.ToMaybe("A");

            Assert.IsTrue(nonNullValue.IsJust);
            Assert.IsFalse(nonNullValue.IsNothing);
        }

        [Test]
        public void Should_Return_Be_Nothing_If_Value_Is_Null()
        {
            var nullValue = Maybe<string>.ToMaybe(null);

            Assert.IsTrue(nullValue.IsNothing);
            Assert.IsFalse(nullValue.IsJust);
        }

        [Test]
        public void Should_Return_A_Just_Value()
        {
            var justValue = Maybe<string>.Just("A");

            Assert.IsTrue(justValue.IsJust);
            Assert.IsFalse(justValue.IsNothing);
        }

        [Test]
        public void Should_Return_A_Nothing_Value()
        {
            var nothingValue = Maybe<string>.Nothing();

            Assert.IsTrue(nothingValue.IsNothing);
            Assert.IsFalse(nothingValue.IsJust);
        }

        [Test]
        public void Should_Return_Wrapped_Value_When_Value_Is_Just()
        {
            var value = Maybe<string>.Just("A").GetOrElse("B");

            Assert.AreEqual(value, "A");
        }

        [Test]
        public void Should_Return_Else_Value_When_Value_Is_Nothing()
        {
            var value = Maybe<string>.Nothing().GetOrElse("B");

            Assert.AreEqual(value, "B");
        }

        [Test]
        public void Should_Return_Mapped_Value_When_Value_Is_Just()
        {
            var mappedValue = Maybe<string>.Just("A").Map<string>(v => "C");

            Assert.AreEqual(mappedValue.GetOrElse("B"), "C");
        }

        [Test]
        public void Should_Return_Nothing_Value_When_Value_Is_Nothing()
        {
            var mappedValue = Maybe<string>.Nothing().Map<string>(v => "C");

            Assert.IsTrue(mappedValue.IsNothing);
        }

        [Test]
        public void Should_Allow_Unwrapping_A_Just_Value()
        {
            var value = Maybe<string>.Just("A").Unwrap();

            Assert.AreEqual(value, "A");
        }

        [Test]
        public void Should_Throw_Exception_If_Just_Value_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Maybe<string>.Just(null));
        }

        [Test]
        public void Should_Throw_Exception_If_Map_Function_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Maybe<string>.Just("A").Map<string>(null));
        }

        [Test]
        public void Should_Throw_Exception_If_Else_Value_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Maybe<string>.Just("A").GetOrElse(null));
        }

        [Test]
        public void Should_Throw_Exception_If_Unwrapping_A_Nothing_Value()
        {
            Assert.Throws<InvalidOperationException>(() => Maybe<string>.Nothing().Unwrap());
        }
    }
}