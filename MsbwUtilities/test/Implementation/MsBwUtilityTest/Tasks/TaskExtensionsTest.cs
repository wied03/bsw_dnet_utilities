﻿#region

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MsBw.MsBwUtility.Tasks;
using NUnit.Framework;

#endregion

namespace MsBwUtilityTest.Tasks
{
    [TestFixture]
    public class TaskExtensionsTest : BaseTest
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        private async Task<int> DelayFor(TimeSpan time)
        {
            await Task.Delay(time);
            return 5;
        }

        [Test]
        public async Task With_timeout_task_finishes()
        {
            // arrange + act
            var result = await this.WithTimeout(t => DelayFor(500.Milliseconds()),
                                                5.Seconds());

            // assert
            result.Should().Be(5);
        }

        [Test]
        public void With_timeout_task_times_out()
        {
            // arrange + act + assert
            this.Invoking(t => t.WithTimeout(ta => ta.DelayFor(1.Seconds()),
                                             200.Milliseconds()).Wait())
                .ShouldThrow<AggregateException>()
                .WithInnerException<TimeoutException>()
                ;
        }

        [Test]
        public async Task Timeout_or_cancel_normal()
        {
            // arrange
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var task = DelayFor(500.Milliseconds());

            // act
            var result = await task.TimeoutOrCancel(token,
                                                    1.Seconds());

            // assert
            result
                .Should()
                .Be(Result.Normal);
        }

        [Test]
        public async Task Timeout_or_cancel_timed_out()
        {
            // arrange
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var task = DelayFor(1.Seconds());

            // act
            var result = await task.TimeoutOrCancel(token,
                                                    200.Milliseconds());

            // assert
            result
                .Should()
                .Be(Result.Timeout);
        }

        [Test]
        public async Task Timeout_or_cancel_canceled()
        {
            // arrange
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var task = DelayFor(10.Seconds());

            // act
            var resultTask = task.TimeoutOrCancel(token,
                                                  20.Seconds());
            await Task.Delay(100.Milliseconds());
            await Task.Factory.StartNew(cts.Cancel);
            var result = await resultTask;

            // assert
            result
                .Should()
                .Be(Result.Canceled);
        }
    }
}