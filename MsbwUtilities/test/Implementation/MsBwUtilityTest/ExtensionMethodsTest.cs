#region

using System;
using System.Linq;
using System.Linq.Expressions;
using MsBw.MsBwUtility;
using NUnit.Framework;
using FluentAssertions;

#endregion

namespace MsBwUtilityTest
{
    [TestFixture]
    public class ExtensionMethodsTest : BaseTest
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void Times_action()
        {
            // arrange
            var times = 0;
            Action foo = () => times++;

            // act
            6.Times(foo);

            // assert
            times
                .Should()
                .Be(6);
        }

        [Test]
        public void Times_specify_value()
        {
            // arrange
            var times = 0;
            Action<int> foo = (i) => times += i+1;

            // act
            3.Times(foo);

            // assert
            times
                .Should()
                .Be(6);
        }

        [Test]
        public void Sequential_array()
        {
            // arrange + act
            var numbers = 5.SequentialArray();

            // assert
            numbers
                .ShouldBeEquivalentTo(new[] {0, 1, 2, 3, 4});
        }

        [Test]
        public void To_byte_array_from_hex_lowercase()
        {
            // arrange
            const string hex = "5a4c";

            // act
            var result = hex.ToByteArrayFromHex();

            // assert
            result
                .ShouldBeEquivalentTo(new byte[] {0x5a, 0x4c});
        }

        [Test]
        public void To_byte_array_from_hex_uppercase()
        {
            // arrange
            const string hex = "5A4C";

            // act
            var result = hex.ToByteArrayFromHex();

            // assert
            result
                .ShouldBeEquivalentTo(new byte[] { 0x5a, 0x4c });
        }

        [Test]
        public void To_hex()
        {
            // arrange
            var bytes = new byte[] {0x5a, 0x4c};

            // act
            var result = bytes.ToHex();

            // assert
            result
                .Should()
                .Be("5A4C");
        }
    }
}