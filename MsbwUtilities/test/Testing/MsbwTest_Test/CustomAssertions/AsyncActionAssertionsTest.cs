// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using MsbwTest;
using MsbwTest.CustomAssertions;
using NUnit.Framework;

namespace MsbwTest_Test.CustomAssertions
{
    [TestFixture]
    public class AsyncActionAssertionsTest : BaseTest
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        async Task Tester(bool throwException)
        {
            if (throwException)
            {
                throw new ArgumentException("foobar");
            }
            await Task.Delay(100.Milliseconds());
        }


        async Task ThrowException()
        {
            await Task.Delay(100.Milliseconds());
            throw new ArgumentException("foobar");
        }

        #endregion

        #region Utility Methods

        #endregion

        #region Tests

        [Test]
        public void Expected_exception_does_not_happen()
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
        public async Task Expected_exception_happens()
        {
            // arrange
            var asyncAssertions = new AsyncActionAssertions(() => Tester(true));

            // act + assert (no exception = pass)
            await asyncAssertions.ShouldThrow<ArgumentException>();
        }

        [Test]
        public async Task Should_complete_within_finishes()
        {
            // arrange
            var asyncAssertions = new AsyncActionAssertions(() => Tester(false));

            // act + assert
            await asyncAssertions.ShouldCompleteWithin(200.Milliseconds());
        }

        [Test]
        public async Task Should_complete_within_doesntfinish()
        {
            // arrange
            var asyncAssertions = new AsyncActionAssertions(() => Tester(false));

            // act + assert
            await asyncAssertions.InvokingAsync(a => a.ShouldCompleteWithin(50.Milliseconds()))
                                 .ShouldThrow<AssertionException>();
        }

        [Test]
        public async Task Should_complete_within_finishes_throws_exception()
        {
            // arrange
            var asyncAssertions = new AsyncActionAssertions(ThrowException);

            // act + assert
            await asyncAssertions.InvokingAsync(a => a.ShouldCompleteWithin(200.Milliseconds()))
                                 .ShouldThrow<ArgumentException>();
        }

        #endregion
    }
}