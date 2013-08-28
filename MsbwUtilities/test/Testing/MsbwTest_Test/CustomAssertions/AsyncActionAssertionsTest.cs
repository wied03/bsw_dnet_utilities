// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
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
        public void Expected_exception_does_not_happen()
        {
            // arrange
            var asyncAssertions = new AsyncActionAssertions(() => Tester(false));

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
            var asyncAssertions = new AsyncActionAssertions(() => Tester(true));

            // act + assert (no exception = pass)
            await asyncAssertions.ShouldThrow<ArgumentException>();
        }
    }
}