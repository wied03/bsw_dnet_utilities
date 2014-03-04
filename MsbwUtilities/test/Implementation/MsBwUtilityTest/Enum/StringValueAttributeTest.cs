// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

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

    public enum TestEnum2
    {
        [StringValue("bar-2")] Value2,
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

        [Test]
        public void No_collision_between_types()
        {
            // arrange
            const TestEnum test = TestEnum.Value2;
            const TestEnum2 test2 = TestEnum2.Value2;

            // act
            var result1 = test.StringValue();
            var result2 = test2.StringValue();

            // assert
            result1
                .Should()
                .Be("bar");
            result2
                .Should()
                .Be("bar-2");
            "bar".EnumValue<TestEnum>()
                 .Should()
                 .Be(TestEnum.Value2);
            "bar-2".EnumValue<TestEnum2>()
                   .Should()
                   .Be(TestEnum2.Value2);
        }
    }
}