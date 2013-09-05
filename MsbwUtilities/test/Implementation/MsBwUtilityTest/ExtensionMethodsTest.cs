// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MsBw.MsBwUtility;
using NUnit.Framework;

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
            Action<int> foo = (i) => times += i + 1;

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
                .ShouldBeEquivalentTo(new byte[] {0x5a, 0x4c});
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

        [Test]
        public void Nullable_int_null()
        {
            // arrange
            string input = null;

            // act
            var result = input.ToNullableInt();

            // assert
            result.Should()
                  .NotHaveValue();
        }

        [Test]
        public void Nullable_int_empty()
        {
            // arrange
            var input = string.Empty;

            // act
            var result = input.ToNullableInt();

            // assert
            result.Should()
                  .NotHaveValue();
        }

        [Test]
        public void Nullable_int_not_null()
        {
            // arrange
            const string input = "2";

            // act
            var result = input.ToNullableInt();

            // assert
            result
                .Should()
                .Be(2);
        }
    }
}