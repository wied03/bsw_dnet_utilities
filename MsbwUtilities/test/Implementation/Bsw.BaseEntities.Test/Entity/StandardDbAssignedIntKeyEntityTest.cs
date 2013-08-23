#region

using System;
using System.Linq;
using System.Linq.Expressions;
using Bsw.BaseEntities.Entity;
using NUnit.Framework;
using FluentAssertions;

#endregion

namespace Bsw.BaseEntities.Test.Entity
{
    public class TestEntityClass : StandardDbAssignedIntKeyEntity<TestEntityClass>
    {
        private string _property1;

        public TestEntityClass(string property1)
        {
            _property1 = property1;
        }

        public virtual string Property1
        {
            get { return _property1; }
            protected set { _property1 = value; }
        }
    }

    [TestFixture]
    public class StandardDbAssignedIntKeyEntityTest : BaseTest
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
        public void Two_transient_objects_equal()
        {
            // arrange

            // act

            // assert
            Assert.Fail("write test");
        }

        [Test]
        public void Two_transient_objects_not_equal()
        {
            // arrange

            // act

            // assert
            Assert.Fail("write test");
        }

        [Test]
        public void First_is_transient_second_isnt()
        {
            // arrange

            // act

            // assert
            Assert.Fail("write test");
        }

        [Test]
        public void First_isnt_transient_second_is()
        {
            // arrange

            // act

            // assert
            Assert.Fail("write test");
        }

        [Test]
        public void Both_not_transient_not_equal()
        {
            // arrange

            // act

            // assert
            Assert.Fail("write test");
        }

        [Test]
        public void Both_not_transient_equal()
        {
            // arrange

            // act

            // assert
            Assert.Fail("write test");
        }

        #endregion
    }
}