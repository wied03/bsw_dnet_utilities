// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using MsBw.MsBwUtility.JetBrains.Annotations;
using MsbwTest;
using NUnit.Framework;
using Rhino.Mocks;

namespace MsbwTest_Test
{
    public interface ITest
    {
        string Prop1 { get; set; }
        int Prop2 { get; set; }
        string Prop3 { get; [UsedImplicitly] set; }
    }

    [TestFixture]
    public class RhinoMocksExtensionsTest : BaseTest
    {
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
        public void Setup_property_behavior_on_all()
        {
            // arrange
            var mock = MockRepository.GenerateMock<ITest>();

            // act
            mock.SetupPropertyBehaviorOnAll();
            mock.Prop1 = "foo";
            mock.Prop2 = 2003;

            // assert
            mock.Prop1
                .Should()
                .Be("foo");
            mock.Prop2
                .Should()
                .Be(2003);
        }

        [Test]
        public void Setup_property_behavior_on_all_except()
        {
            // arrange
            var mock = MockRepository.GenerateMock<ITest>();

            // act
            mock.SetupPropertyBehaviorOnAllExcept(c => c.Prop1,
                                                  c => c.Prop3);
            mock.Prop2 = 9999;
            mock.Stub(c => c.Prop1)
                .Return("foobar");
            mock.Stub(c => c.Prop3)
                .Return("bar");

            // assert
            mock.Prop1
                .Should()
                .Be("foobar");
            mock.Prop2
                .Should()
                .Be(9999);
            mock.Prop3
                .Should()
                .Be("bar");
        }

        // Rhino Mocks / Castle Proxy need this to be pub
// ReSharper disable once MemberCanBePrivate.Global
        public interface ITestDoAndReturn
        {
            Task NoReturnValue();
            Task<string> OneReturnValue();
            Task NoReturnParam1(string stuff);

            Task NoReturnParam2(string stuff,
                                int otherStuff);

            Task<string> ReturnWithParam1(int stuff);

            Task<string> ReturnWithParam2(int stuff,
                                          string otherStuff);
        }

        private static ITestDoAndReturn Mock
        {
            get { return MockRepository.GenerateMock<ITestDoAndReturn>(); }
        }

        [Test]
        public async Task Return_async_return_val()
        {
            // arrange
            var mock = Mock;
            mock.Stub(m => m.OneReturnValue())
                .IgnoreArguments()
                .ReturnAsync("stuff");

            // act
            var result = await mock.OneReturnValue();

            // assert
            result
                .Should()
                .Be("stuff");
        }

        [Test]
        public async Task Return_async_return_val_and_param()
        {
            // arrange
            var mock = Mock;
            mock.Stub(m => m.ReturnWithParam1(5))
                .IgnoreArguments()
                .ReturnAsync("stuff");

            // act
            var result = await mock.ReturnWithParam1(55);

            // assert
            result
                .Should()
                .Be("stuff",
                    "shouldn't care about arguments when using returnasync");
        }

        [Test]
        public async Task Do_async_void_no_params()
        {
            // arrange
            var mock = Mock;
            var invoked = false;
            mock.Stub(m => m.NoReturnValue())
                .DoAsyncVoid(() => invoked = true);

            // act
            await mock.NoReturnValue();

            // assert
            invoked
                .Should()
                .BeTrue();
        }

        [Test]
        public async Task Do_async_void_1_param()
        {
            // arrange
            var mock = Mock;
            string capturedStr = null;
            mock.Stub(m => m.NoReturnParam1(null))
                .IgnoreArguments()
                .DoAsyncVoid<string>(s => capturedStr = s);

            // act
            await mock.NoReturnParam1("the stuff");

            // assert
            capturedStr
                .Should()
                .Be("the stuff");
        }

        [Test]
        public async Task Do_async_void_2_params()
        {
            // arrange
            var mock = Mock;
            string capturedStr = null;
            int? capturedVal = null;
            mock.Stub(m => m.NoReturnParam2(null,
                                            5))
                .IgnoreArguments()
                .DoAsyncVoid<string, int>((s,
                                           i) =>
                                          {
                                              capturedStr = s;
                                              capturedVal = i;
                                          });

            // act
            await mock.NoReturnParam2("foo",
                                      99);

            // assert
            capturedStr
                .Should()
                .Be("foo");
            capturedVal
                .Should()
                .Be(99);
        }

        [Test]
        public async Task Stub_async_void_with_params()
        {
            // arrange
            var mock = Mock;
            mock.StubAsyncVoid(m => m.NoReturnParam1(null))
                .IgnoreArguments();

            // act + assert
            await mock.NoReturnParam1("stuff");
        }

        [Test]
        public async Task Stub_async_void_no_params()
        {
            // arrange
            var mock = Mock;
            mock.StubAsyncVoid(m => m.NoReturnValue())
                .IgnoreArguments();

            // act + assert
            await mock.NoReturnValue();
        }

        [Test]
        [Category("Cat1")]
        [Category("Cat2")]
        public async Task Do_async_return_value()
        {
            // arrange
            var mock = Mock;

            // act
            mock.Stub(m => m.OneReturnValue())
                .DoAsync(() => "hi there");
            var result = await mock.OneReturnValue();

            // assert
            result
                .Should()
                .Be("hi there");
        }

        [Test]
        public async Task Do_async_return_value_timed()
        {
            // arrange
            var mock = Mock;
            var sw = new Stopwatch();

            // act
            mock.Stub(m => m.OneReturnValue())
                .DoAsync(() => "hi there",
                         3.Seconds());
            sw.Start();
            var result = await mock.OneReturnValue();
            sw.Stop();

            // assert
            sw.Elapsed
              .Should()
              .BeGreaterOrEqualTo(3.Seconds())
              .And
              .BeLessOrEqualTo(4.Seconds())
                ;
            result.Should()
                  .Be("hi there");
        }

        [Test]
        public async Task Do_async_return_value_1_parameter()
        {
            // arrange
            var mock = Mock;
            int? invokedWithNumber = null;

            // act
            mock.Stub(m => m.ReturnWithParam1(33))
                .IgnoreArguments()
                .DoAsync<int, string>(num =>
                                      {
                                          invokedWithNumber = num;
                                          return "hi there";
                                      });
            var result = await mock.ReturnWithParam1(52);

            // assert
            invokedWithNumber
                .Should()
                .Be(52);
            result
                .Should()
                .Be("hi there");
        }

        [Test]
        public async Task Do_async_return_value_1_parameter_timed()
        {
            // arrange
            var mock = Mock;
            int? invokedWithNumber = null;
            var sw = new Stopwatch();

            // act
            mock.Stub(m => m.ReturnWithParam1(33))
                .IgnoreArguments()
                .DoAsync<int, string>(num =>
                                      {
                                          invokedWithNumber = num;
                                          return "hi there";
                                      },
                                      3.Seconds());
            sw.Start();
            var result = await mock.ReturnWithParam1(52);
            sw.Stop();

            // assert
            sw.Elapsed
              .Should()
              .BeGreaterOrEqualTo(3.Seconds())
              .And
              .BeLessOrEqualTo(4.Seconds())
                ;
            invokedWithNumber
                .Should()
                .Be(52);
            result
                .Should()
                .Be("hi there");
        }

        [Test]
        public async Task Do_async_return_value_2_parameters()
        {
            // arrange
            var mock = Mock;
            int? invokedWithNumber = null;
            string invokedWithString = null;

            // act
            mock.Stub(m => m.ReturnWithParam2(55,
                                              null))
                .IgnoreArguments()
                .DoAsync<int, string, string>((num,
                                               str) =>
                                              {
                                                  invokedWithNumber = num;
                                                  invokedWithString = str;
                                                  return "foobar";
                                              });
            var result = await mock.ReturnWithParam2(95,
                                                     "hi there");

            // assert
            result
                .Should()
                .Be("foobar");
            invokedWithNumber
                .Should()
                .Be(95);
            invokedWithString
                .Should()
                .Be("hi there");
        }

        [Test]
        public async Task Do_async_return_value_2_parameters_timed()
        {
            // arrange
            var mock = Mock;
            int? invokedWithNumber = null;
            string invokedWithString = null;
            var sw = new Stopwatch();

            // act
            mock.Stub(m => m.ReturnWithParam2(55,
                                              null))
                .IgnoreArguments()
                .DoAsync<int, string, string>((num,
                                               str) =>
                                              {
                                                  invokedWithNumber = num;
                                                  invokedWithString = str;
                                                  return "foobar";
                                              },
                                              3.Seconds());

            sw.Start();
            var result = await mock.ReturnWithParam2(95,
                                                     "hi there");
            sw.Stop();

            // assert
            sw.Elapsed
              .Should()
              .BeGreaterOrEqualTo(3.Seconds())
              .And
              .BeLessOrEqualTo(4.Seconds())
                ;
            result
                .Should()
                .Be("foobar");
            invokedWithNumber
                .Should()
                .Be(95);
            invokedWithString
                .Should()
                .Be("hi there");
        }

        #endregion
    }
}