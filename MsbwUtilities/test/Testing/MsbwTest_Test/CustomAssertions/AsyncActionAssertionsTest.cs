﻿#region

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MsbwTest.CustomAssertions;
using NUnit.Framework;
using FluentAssertions;

#endregion

namespace MsbwTest_Test.CustomAssertions
{
    [TestFixture]
    public class AsyncActionAssertionsTest : BaseTest
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        private async Task<int> Tester(bool throwException)
        {
            await Task.Delay(100.Milliseconds());
            if (throwException)
                throw new ArgumentException("foobar");
            return 5;
        }

        [Test]
        public void Should_throw_no_exception()
        {
            // arrange
            var asyncAssertions = new AsyncActionAssertions(() => Tester(false));

            // act + assert
            asyncAssertions
                .Invoking(a => a.ShouldThrow<ArgumentException>().Wait())
                .ShouldThrow<AggregateException>()
                .WithInnerException<AssertionException>()
                ;
        }

        [Test]
        public async Task Should_throw_exception()
        {
            // arrange
            var asyncAssertions = new AsyncActionAssertions(() => Tester(true));

            // act +assert (no exception = pass)
            await asyncAssertions.ShouldThrow<ArgumentException>();
        }
    }
}