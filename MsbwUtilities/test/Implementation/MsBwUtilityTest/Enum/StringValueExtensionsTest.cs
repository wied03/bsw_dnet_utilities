// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MsBw.MsBwUtility.Enum;
using NUnit.Framework;

namespace MsBwUtilityTest.Enum
{
    [TestFixture]
    public class StringValueExtensionsTest : BaseTest
    {
        public enum TesterForInt
        {
            Value1,
            Value2
        }

        public enum TesterForString
        {
            Value1,
            Value2
        }

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        #endregion

        #region Utility Methods

        #endregion

        #region Tests

        [Test]
        public void To_mapped_enum_value_string_found()
        {
            // arrange
            var dict = new Dictionary<string, TesterForString>
                       {
                           {"test", TesterForString.Value1}
                       };

            // act
            var result = dict.ToMappedEnumValue("test");

            // assert
            result
                .Should()
                .Be(TesterForString.Value1);
        }

        [Test]
        public void To_mapped_enum_value_string_notfound()
        {
            // arrange
            var dict = new Dictionary<string, TesterForString>
                       {
                           {"test", TesterForString.Value1}
                       };

            // act + assert
            dict.Invoking(d => d.ToMappedEnumValue("foo"))
                .ShouldThrow<EnumNotFoundException>();
        }

        [Test]
        public void To_mapped_enum_value_int_found()
        {
            // arrange
            var dict = new Dictionary<int, TesterForInt>
                       {
                           {2, TesterForInt.Value1}
                       };

            // act
            var result = dict.ToMappedEnumValue(2);

            // assert
            result
                .Should()
                .Be(TesterForInt.Value1);
        }

        [Test]
        public void To_mapped_enum_value_int_notfound()
        {
            // arrange
            var dict = new Dictionary<int, TesterForInt>
                       {
                           {2, TesterForInt.Value1}
                       };

            // act + assert
            dict.Invoking(d => d.ToMappedEnumValue(33))
                .ShouldThrow<EnumNotFoundException>();
        }

        [Test]
        public void From_mapped_enum_value_string_found()
        {
            // arrange
            var dict = new Dictionary<string, TesterForString>
                       {
                           {"test", TesterForString.Value1}
                       };

            // act
            var result = dict.FromMappedEnumValue(TesterForString.Value1);

            // assert
            result
                .Should()
                .Be("test");
        }

        [Test]
        public void From_mapped_enum_value_string_notfound()
        {
            // arrange
            var dict = new Dictionary<string, TesterForString>
                       {
                           {"test", TesterForString.Value1}
                       };

            // act + assert
            dict.Invoking(d => d.FromMappedEnumValue(TesterForString.Value2))
                .ShouldThrow<EnumNotFoundException>();
        }

        [Test]
        public void From_mapped_enum_value_int_found()
        {
            // arrange
            var dict = new Dictionary<int, TesterForInt>
                       {
                           {3, TesterForInt.Value1}
                       };

            // act
            var result = dict.FromMappedEnumValue(TesterForInt.Value1);

            // assert
            result
                .Should()
                .Be(3);
        }

        [Test]
        public void From_mapped_enum_value_int_notfound()
        {
            // arrange
            var dict = new Dictionary<int, TesterForInt>
                       {
                           {3, TesterForInt.Value1}
                       };

            // act + assert
            dict.Invoking(d => d.FromMappedEnumValue(TesterForInt.Value2))
                .ShouldThrow<EnumNotFoundException>();
        }

        #endregion
    }
}