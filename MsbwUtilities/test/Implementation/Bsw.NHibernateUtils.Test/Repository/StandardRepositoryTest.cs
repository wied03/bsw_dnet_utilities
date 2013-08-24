#region

using System;
using System.Linq;
using System.Linq.Expressions;
using Bsw.NHibernateUtils.Repository;
using Bsw.NHibernateUtils.Test.TestEntities;
using NHibernate;
using NUnit.Framework;
using FluentAssertions;

#endregion

namespace Bsw.NHibernateUtils.Test.Repository
{
    [TestFixture]
    public class StandardRepositoryTest : BaseDbDrivenTest
    {
        private UnitOfWork _unitOfWork;
        private StandardRepository<EntityClass1> _repo;

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _unitOfWork = new UnitOfWork(SessionFactory);
            _repo = new StandardRepository<EntityClass1>(_unitOfWork);
        }

        [TearDown]
        public override void TearDown()
        {
            _unitOfWork.Dispose();
            base.TearDown();
        }

        #endregion

        #region Utility Methods

        #endregion

        #region Tests

        [Test]
        public void Default_transaction_rollback_on_error()
        {
            // arrange
            var obj = new EntityClass1 {Item3 = "foobar"};
            var transaction = _unitOfWork.CurrentSession.Transaction;

            // act
            _repo.Invoking(r => r.Update(obj))
                 .ShouldThrow<TransientObjectException>();

            // assert
            transaction.WasRolledBack
                       .Should()
                       .BeTrue();
        }

        [Test]
        public void Transaction_rollback_turned_off_on_error()
        {
            // arrange
            var obj = new EntityClass1 { Item3 = "foobar" };
            var transaction = _unitOfWork.CurrentSession.Transaction;
            _repo.TransactionActionOnError = TransactionActionOnError.Nothing;

            // act
            _repo.Invoking(r => r.Update(obj))
                 .ShouldThrow<TransientObjectException>();

            // assert
            transaction.WasRolledBack
                       .Should()
                       .BeFalse();
        }

        #endregion
    }
}