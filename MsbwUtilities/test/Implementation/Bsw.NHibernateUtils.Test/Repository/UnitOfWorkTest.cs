#region

using System;
using System.Linq;
using System.Linq.Expressions;
using Bsw.NHibernateUtils.Repository;
using Bsw.NHibernateUtils.Test.TestEntities;
using NUnit.Framework;
using FluentAssertions;

#endregion

namespace Bsw.NHibernateUtils.Test.Repository
{
    [TestFixture]
    public class UnitOfWorkTest : BaseDbDrivenTest
    {
        private IUnitOfWork _unitOfWork;
        private IStandardRepository<EntityClass1> _repo;

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _unitOfWork = new UnitOfWork(SessionFactory);
            _repo = new StandardRepository<EntityClass1>(_unitOfWork);
        }

        #endregion

        #region Utility Methods

        #endregion

        #region Tests

        [Test]
        public void Session_never_utilized()
        {
            // arrange

            // act + assert
            SessionFactory.Statistics.SessionOpenCount.Should().Be(0);
            _unitOfWork.Dispose();

            // assert
            SessionFactory.Statistics.SessionOpenCount.Should().Be(0);
        }

        [Test]
        public void Explicit_commit_normal()
        {
            // arrange
            var obj1 = new EntityClass1 {Item4 = "foobar"};
            _repo.SaveOrUpdate(obj1);
            var originalTransaction = _unitOfWork.CurrentSession.Transaction;

            // act
            _unitOfWork.Commit();

            // assert
            obj1.Id
                .Should()
                .Be(1);
            
            _unitOfWork.CurrentSession.Transaction
                       .Should()
                       .NotBe(originalTransaction);
            var entity = _repo.Query().First();
            entity.ShouldBeEquivalentTo(obj1);
        }

        [Test]
        public void Explicit_commit_do_not_open_new_transaction()
        {
            // arrange
            var obj1 = new EntityClass1 { Item4 = "foobar" };
            _repo.SaveOrUpdate(obj1);

            // act
            _unitOfWork.Commit(openNewTransactionAfterCommittingCurrent: false);

            // assert
            _unitOfWork.CurrentSession.Transaction
                       .WasCommitted
                       .Should()
                       .BeTrue();
        }

        [Test]
        public void Using_based_commit_normal()
        {
            var obj1 = new EntityClass1 { Item4 = "foobar" };
            using (var uow = new UnitOfWork(SessionFactory))
            {
                // arrange
                var repo = new StandardRepository<EntityClass1>(uow);

                // act
                repo.SaveOrUpdate(obj1);
            }

            // assert
            var entity = _repo.Query().First();
            entity.ShouldBeEquivalentTo(obj1);
        }

        [Test]
        public void Explicit_commit_fails()
        {
            // arrange

            // act

            // assert
            Assert.Fail("write test");
        }

        [Test]
        public void Using_based_commit_fails()
        {
            // arrange

            // act

            // assert
            Assert.Fail("write test");
        }

        [Test]
        public void Rollback_successfully()
        {
            // arrange

            // act

            // assert
            Assert.Fail("write test");
        }

        #endregion
    }
}