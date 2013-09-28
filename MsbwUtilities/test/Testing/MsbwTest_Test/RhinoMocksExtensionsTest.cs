// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using MsbwTest;
using NUnit.Framework;
using FluentAssertions;
using Rhino.Mocks;

namespace MsbwTest_Test
{
    public interface ITest
    {
        string Prop1 { get; set; }
        int Prop2 { get; set; }
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

        #endregion
    }
}