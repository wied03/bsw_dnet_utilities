// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MsbwTest;
using NUnit.Framework;

namespace MsbwTest_Test
{
    [TestFixture]
    public class ExtensionMethodsTest : BaseTest
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        #endregion

        #region Utility Methods

        private async Task DoStuff()
        {
            throw new Exception("foobar");
        }

        private async Task<int> DoOtherStuff()
        {
            throw new Exception("foobar");
        }

        private async Task DoSomethingForTime(TimeSpan time)
        {
            await Task.Delay(time);
        }

        private Task NotAsyncTaskMethod()
        {
            return Task.Factory.StartNew(() =>
                                         {
                                             Thread.Sleep(50.Milliseconds());
                                             throw new ArgumentException("foobar");
                                         });
        }

        private interface ITest
        {
            Task NotAsyncTaskMethodImplementedAsAsync();
        }

        private class Test : ITest
        {
            public async Task NotAsyncTaskMethodImplementedAsAsync()
            {
                await Task.Delay(50.Milliseconds());
                throw new ArgumentException("foobar");
            }
        }

        #endregion

        #region Tests

        [Test]
        public async Task Invoking_async_void()
        {
            // arrange + act + assert
            await this.InvokingAsync(t => t.DoStuff())
                      .ShouldThrow<Exception>();
        }

        [Test]
        public async Task Invoking_async_void_withnonasync_task_method()
        {
            // arrange + act + assert
            await this.InvokingAsync(t => t.NotAsyncTaskMethod())
                      .ShouldThrow<ArgumentException>();
        }

        [Test]
        public async Task Invoking_async_void_withnonasync_interface_task_method_impl_as_async()
        {
            // arrange
            ITest test = new Test();
 
            // act + assert
            await test.InvokingAsync(t => t.NotAsyncTaskMethodImplementedAsAsync())
                      .ShouldThrow<ArgumentException>();
        }

        [Test]
        public async Task Invoking_async_notvoid_exception()
        {
            // arrange + act + assert
            await this.InvokingAsync(t => t.DoOtherStuff())
                      .ShouldThrow<Exception>();
        }

        [Test]
        public async Task Should_complete_within_tcs_pass()
        {
            // arrange
            var tcs = new TaskCompletionSource<int>();
            await Task.Factory.StartNew(() =>
                                        {
                                            Task.Delay(20.Milliseconds());
                                            tcs.SetResult(5);
                                        });

            // act
            var result = await tcs.ShouldCompleteWithin(100.Milliseconds());

            // assert
            result
                .Should()
                .Be(5);
        }

        [Test]
        public async Task Should_complete_within_tcs_fail()
        {
            // arrange
            var tcs = new TaskCompletionSource<int>();
            await Task.Factory.StartNew(async () =>
            {
                await Task.Delay(3.Seconds());
                tcs.SetResult(5);
            });

            // act + assert
            await tcs.InvokingAsync(a => a.ShouldCompleteWithin(50.Milliseconds()))
                     .ShouldThrow<AssertionException>();
        }

        [Test]
        public async Task Should_complete_within_task_pass()
        {
            // arrange
            var tcs = new TaskCompletionSource<int>();
            await Task.Factory.StartNew(() =>
                                        {
                                            Task.Delay(20.Milliseconds());
                                            tcs.SetResult(5);
                                        });

            // act
            var result = await tcs.Task.ShouldCompleteWithin(100.Milliseconds());

            // assert
            result
                .Should()
                .Be(5);
        }

        [Test]
        public async Task Should_complete_within_task_fail()
        {
            // arrange
            var tcs = new TaskCompletionSource<int>();
            await Task.Factory.StartNew(async () =>
                                        {
                                            await Task.Delay(3.Seconds());
                                            tcs.SetResult(5);
                                        });

            // act + assert
            await tcs.Task.InvokingAsync(a => a.ShouldCompleteWithin(20.Milliseconds()))
                     .ShouldThrow<AssertionException>();
        }

        [Test]
        public async Task Should_complete_within_task_void_pass()
        {
            // arrange
            var task = DoSomethingForTime(50.Milliseconds());

            // act + assert (quiet is good)
            await task.ShouldCompleteWithin(80.Milliseconds());
        }

        [Test]
        public async Task Should_complete_within_task_void_fail()
        {
            // arrange
            var task = DoSomethingForTime(50.Milliseconds());

            // act + assert
            try
            {
                await task.ShouldCompleteWithin(20.Milliseconds(),
                                                "i said so");
                Assert.Fail("Expected exception");
            }
            catch (AssertionException)
            {
                Assert.Pass("Got our exception");
            }
        }

        #endregion
    }
}