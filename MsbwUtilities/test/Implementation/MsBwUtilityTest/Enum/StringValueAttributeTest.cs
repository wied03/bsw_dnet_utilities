#region

using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MsBw.MsBwUtility.Enum;
using NUnit.Framework;

#endregion

namespace MsBwUtilityTest.Enum
{
    public enum TestEnum
    {
        [StringValue("foo")] Value,
        [StringValue("bar")] Value2,
        Value3
    }

    [TestFixture]
    public class StringValueAttributeTest : BaseTest
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void Enum_to_string()
        {
            // arrange
            const TestEnum test = TestEnum.Value;

            // act
            var result = test.StringValue();

            // assert
            result
                .Should()
                .Be("foo");
        }

        [Test]
        public void String_to_enum()
        {
            // arrange
            const string test = "bar";

            // act
            var result = test.EnumValue<TestEnum>();

            // assert
            result
                .Should()
                .Be(TestEnum.Value2);
        }

        [Test]
        public void String_to_enum_not_found()
        {
            // arrange
            const string test = "blah";

            // act + assert
            test.Invoking(t => t.EnumValue<TestEnum>())
                .ShouldThrow<EnumNotFoundException>()
                ;
        }

        [Test]
        public void String_to_enum_default()
        {
            // arrange
            const string test = "Value3";

            // act
            var result = test.EnumValue<TestEnum>();

            // assert
            result
                .Should()
                .Be(TestEnum.Value3);
        }

        [Test]
        public void Enum_to_string_default()
        {
            const TestEnum test = TestEnum.Value3;

            // act
            var result = test.StringValue();

            // assert
            result
                .Should()
                .Be("Value3");
        }
    }
}