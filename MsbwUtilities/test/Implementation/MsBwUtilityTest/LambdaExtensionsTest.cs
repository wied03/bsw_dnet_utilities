// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MsBw.MsBwUtility;
using NUnit.Framework;
using FluentAssertions;

namespace MsBwUtilityTest
{
    public class TestClass
    {
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public int? ValueNullable { get; set; }
        public int Value { get; set; }
        public string Ref { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }

    [TestFixture]
    public class LambdaExtensionsTest : BaseTest
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        #endregion

        #region Utility Methods

        private static PropertyInfo Test(Expression<Func<TestClass, object>> lambda)
        {
            return lambda.ToPropertyInfo();
        }

        #endregion

        #region Tests

        [Test]
        public void Value_nullable()
        {
            // arrange + act
            var result = Test(p => p.ValueNullable);

            // assert
            result.Name.Should()
                  .Be("ValueNullable");
        }

        [Test]
        public void Value()
        {
            // arrange + act
            var result = Test(p => p.Value);

            // assert
            result.Name.Should()
                  .Be("Value");
        }

        [Test]
        public void Ref()
        {
            // arrange + act
            var result = Test(p => p.Ref);

            // assert
            result.Name.Should()
                  .Be("Ref");
        }

        #endregion
    }
}