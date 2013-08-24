#region

using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using FluentAssertions;

#endregion

namespace Bsw.NHibernateUtils.Test.Repository
{
    [TestFixture]
    public class StandardRepositoryTest : BaseTest
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
        public void THETEST()
        {
            // arrange

            // act

            // assert
            Assert.Fail("write test, probably want to allow the user to control whether transactions are rolled back within the repo when an exception happens.  By default, roll them back");
        }

        #endregion
    }
}