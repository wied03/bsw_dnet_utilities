// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Linq;
using System.Linq.Expressions;
using Bsw.BaseEntities.Entity;
using NUnit.Framework;

#endregion

namespace Bsw.BaseEntities.Test.Entity
{
    public class TestEntityClass : StandardDbAssignedIntKeyEntity<TestEntityClass>
    {
    }

    public sealed class NHibernateStyleProxy : TestEntityClass
    {
        public NHibernateStyleProxy(int id)
        {
            Id = id;
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
            var entity1 = new TestEntityClass();

            // act
            // this is on purpose
            // ReSharper disable EqualExpressionComparison
            var result = entity1.Equals(entity1);
            var result2 = entity1 == entity1;
            // ReSharper restore EqualExpressionComparison
            var hashCode = entity1.GetHashCode();

            // assert
            result
                .Should()
                .BeTrue();
            result2
                .Should()
                .BeTrue();
            hashCode
                .Should()
                .BeGreaterThan(0);
        }

        [Test]
        public void Two_transient_objects_not_equal()
        {
            // arrange
            var entity1 = new TestEntityClass();
            var entity2 = new TestEntityClass();

            // act
            var result = entity1.Equals(entity2);
            var result2 = entity1 == entity2;
            var hashCode1 = entity1.GetHashCode();
            var hashCode2 = entity2.GetHashCode();

            // assert
            result
                .Should()
                .BeFalse();
            result2
                .Should()
                .BeFalse();
            hashCode1
                .Should()
                .NotBe(hashCode2);
        }

        [Test]
        public void First_is_transient_second_isnt()
        {
            // arrange
            var entity1 = new TestEntityClass();
            var entity2 = new NHibernateStyleProxy(id: 2);

            // act
            var result = entity1.Equals(entity2);
            var result2 = entity1 == entity2;
            var hashCode1 = entity1.GetHashCode();
            var hashCode2 = entity2.GetHashCode();

            // assert
            result
                .Should()
                .BeFalse();
            result2
                .Should()
                .BeFalse();
            hashCode1
                .Should()
                .BeGreaterThan(0);
            hashCode2
                .Should()
                .Be(2.GetHashCode());
        }

        [Test]
        public void First_isnt_transient_second_is()
        {
            // arrange
            var entity2 = new TestEntityClass();
            var entity1 = new NHibernateStyleProxy(id: 2);

            // act
            var result = entity1.Equals(entity2);
            var result2 = entity1 == entity2;
            var hashCode1 = entity1.GetHashCode();
            var hashCode2 = entity2.GetHashCode();

            // assert
            result
                .Should()
                .BeFalse();
            result2
                .Should()
                .BeFalse();
            hashCode2
                .Should()
                .BeGreaterThan(0);
            hashCode1
                .Should()
                .Be(2.GetHashCode());
        }

        [Test]
        public void Both_not_transient_not_equal()
        {
            // arrange
            var entity1 = new NHibernateStyleProxy(id: 2);
            var entity2 = new NHibernateStyleProxy(id: 3);

            // act
            var result = entity1.Equals(entity2);
            var result2 = entity1 == entity2;
            var hashCode1 = entity1.GetHashCode();
            var hashCode2 = entity2.GetHashCode();

            // assert
            result
                .Should()
                .BeFalse();
            result2
                .Should()
                .BeFalse();
            hashCode2
                .Should()
                .Be(3.GetHashCode());
            hashCode1
                .Should()
                .Be(2.GetHashCode());
        }

        [Test]
        public void Both_not_transient_equal()
        {
            // arrange
            var entity1 = new NHibernateStyleProxy(id: 2);
            var entity2 = new NHibernateStyleProxy(id: 2);

            // act
            var result = entity1.Equals(entity2);
            var result2 = entity1 == entity2;
            var hashCode1 = entity1.GetHashCode();
            var hashCode2 = entity2.GetHashCode();

            // assert
            result
                .Should()
                .BeTrue();
            result2
                .Should()
                .BeTrue();
            hashCode2
                .Should()
                .Be(2.GetHashCode());
            hashCode1
                .Should()
                .Be(2.GetHashCode());
        }

        #endregion
    }
}