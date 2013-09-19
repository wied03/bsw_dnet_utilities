// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
﻿#region

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MsbwTest;
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
        public void Expected_exception_does_not_happen()
        {
            // arrange
            var asyncAssertions = new AsyncActionAssertions<int>(() => Tester(false));

            // act + assert
            asyncAssertions
                .Invoking(a => a.ShouldThrow<ArgumentException>().Wait())
                .ShouldThrow<AssertionException>()
                ;
        }

        [Test]
        public async Task Expected_exception_happens()
        {
            // arrange
            var asyncAssertions = new AsyncActionAssertions<int>(() => Tester(true));

            // act + assert (no exception = pass)
            await asyncAssertions.ShouldThrow<ArgumentException>();
        }

        [Test]
        public async Task Should_complete_within_finishes()
        {
            // arrange
            var asyncAssertions = new AsyncActionAssertions<int>(() => Tester(false));

            // act
            var result = await asyncAssertions.ShouldCompleteWithin(200.Milliseconds());

            // assert
            result
                .Should()
                .Be(5);
        }

        [Test]
        public async Task Should_complete_within_doesntfinish()
        {
            // arrange
            var asyncAssertions = new AsyncActionAssertions<int>(() => Tester(false));

            // act + assert
            await asyncAssertions.InvokingAsync(a => a.ShouldCompleteWithin(50.Milliseconds()))
                                 .ShouldThrow<AssertionException>();
        }

        [Test]
        public async Task Should_complete_within_doesntfinish_throws_exception()
        {
            // arrange
            var asyncAssertions = new AsyncActionAssertions<int>(() => Tester(true));

            // act + assert
            await asyncAssertions.InvokingAsync(a => a.ShouldCompleteWithin(200.Milliseconds()))
                                 .ShouldThrow<ArgumentException>();
        }
    }
}