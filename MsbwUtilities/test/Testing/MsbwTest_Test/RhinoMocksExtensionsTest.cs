// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
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

        #endregion
    }
}