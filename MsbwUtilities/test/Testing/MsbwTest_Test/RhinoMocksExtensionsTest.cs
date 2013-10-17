// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using MsbwTest;
using NUnit.Framework;
using Rhino.Mocks;

namespace MsbwTest_Test
{
    public interface ITest
    {
        string Prop1 { get; set; }
        int Prop2 { get; set; }
        string Prop3 { get; set; }
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

        public interface ITestDoAndReturn
        {
            Task NoReturnValue();
            Task<string> OneReturnValue();
            Task NoReturnParam1(string stuff);

            Task NoReturnParam2(string stuff,
                                int otherStuff);
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

        #endregion
    }
}