// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
﻿#region

using System;
using System.Linq;
using System.Linq.Expressions;
using Bsw.NHibernateUtils.Repository;
using Bsw.NHibernateUtils.Test.TestEntities;
using FluentAssertions;
using NHibernate;
using NUnit.Framework;

#endregion

namespace Bsw.NHibernateUtils.Test.Repository
{
    public class SimulateCommitFailureUow : UnitOfWork
    {
        public SimulateCommitFailureUow(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            FailNextCommit = false;
        }

        public bool FailNextCommit { get; set; }

        public override void Commit(bool openNewTransactionAfterCommittingCurrent = true)
        {
            if (FailNextCommit)
            {
                throw new Exception("simulated commit failure");
            }
            base.Commit(openNewTransactionAfterCommittingCurrent);
        }
    }

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
        public void Successful_operation_followed_by_unsuccessful_with_no_auto_rollback()
        {
            // arrange
            var uow = new SimulateCommitFailureUow(SessionFactory);
            var repo = new StandardRepository<EntityClass1>(uow)
                       {
                           TransactionActionOnError = TransactionActionOnError.Nothing
                       };
            var obj1 = new EntityClass1 {Item3 = "foo"};
            repo.SaveOrUpdate(obj1);
            var obj2 = new EntityClass1 {Item3 = "bar"};
            uow.FailNextCommit = true;
            try
            {
                // act + assert
                try
                {
                    repo.Invoking(r => r.Update(obj2))
                        .ShouldThrow<TransientObjectException>(reason: "we haven't saved it first");
                }
                finally
                {
                    uow.Invoking(u => u.Dispose())
                       .ShouldThrow<Exception>("simulated commit failure");
                }
            }
            finally
            {
                uow.CurrentSession.Close();
            }
        }

        [Test]
        public void Successful_operation_followed_by_unsuccessful_with_auto_rollback()
        {
            // arrange
            var uow = new SimulateCommitFailureUow(SessionFactory);
            var repo = new StandardRepository<EntityClass1>(uow);
            var obj1 = new EntityClass1 { Item3 = "foo" };
            repo.SaveOrUpdate(obj1);
            var obj2 = new EntityClass1 { Item3 = "bar" };
            uow.FailNextCommit = true;

            // act + assert
            try
            {
                repo.Invoking(r => r.Update(obj2))
                    .ShouldThrow<TransientObjectException>(reason: "we haven't saved it first");
            }
            finally
            {
                uow.Dispose();
            }
        }

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
            var obj1 = new EntityClass1 { Item4 = "Explicit_commit_normal" };
            _repo.SaveOrUpdate(obj1);
            var originalTransaction = _unitOfWork.CurrentSession.Transaction;

            // act
            _unitOfWork.Commit();

            // assert
            obj1.Id
                .Should()
                .HaveValue();

            _unitOfWork.CurrentSession.Transaction
                       .Should()
                       .NotBe(originalTransaction);
            var entity = _repo.Query().First(e => e.Item4 == "Explicit_commit_normal");
            entity.ShouldBeEquivalentTo(obj1);
        }

        [Test]
        public void Explicit_commit_do_not_open_new_transaction()
        {
            // arrange
            var obj1 = new EntityClass1 {Item4 = "foobar"};
            _repo.SaveOrUpdate(obj1);
            var transaction = _unitOfWork.CurrentSession.Transaction;

            // act
            _unitOfWork.Commit(openNewTransactionAfterCommittingCurrent: false);

            // assert
            transaction
                       .WasCommitted
                       .Should()
                       .BeTrue();
        }

        [Test]
        public void Using_based_commit_normal()
        {
            var obj1 = new EntityClass1 { Item4 = "Using_based_commit_normal" };
            using (var uow = new UnitOfWork(SessionFactory))
            {
                // arrange
                var repo = new StandardRepository<EntityClass1>(uow);

                // act
                repo.SaveOrUpdate(obj1);
            }

            // assert
            var entity = _repo.Query().First(e => e.Item4 == "Using_based_commit_normal");
            entity.ShouldBeEquivalentTo(obj1);
        }

        [Test]
        public void Failure_without_using()
        {
            // arrange
            var obj1 = new EntityClass1 {Item4 = "foobar"};

            // act + assert
            _repo.Invoking(r => r.Update(obj1))
                 .ShouldThrow<TransientObjectException>();
        }

        [Test]
        public void Failure_with_using()
        {
            // arrange
            var obj1 = new EntityClass1 {Item4 = "foobar"};

            // act + assert
            using (var uow = new UnitOfWork(SessionFactory))
            {
                var repo = new StandardRepository<EntityClass1>(uow);
                repo.Invoking(r => r.Update(obj1))
                    .ShouldThrow<TransientObjectException>();
            }
        }

        [Test]
        public void Table_lock_handled_ok()
        {
            // arrange
            var obj1 = new EntityClass1 {Item3 = "foo"};
            using (var uow = new UnitOfWork(SessionFactory))
            {
                var repo = new StandardRepository<EntityClass1>(uow);
                repo.SaveOrUpdate(obj1);
            }

            using (var uowForCommitConflict = new UnitOfWork(SessionFactory))
            {
                var repoForCommitConflict = new StandardRepository<EntityClass1>(uowForCommitConflict);
                obj1.Item3 = "modified";
                _repo.SaveOrUpdate(obj1);
                repoForCommitConflict.Invoking(r => r.SaveOrUpdate(obj1))
                                     .ShouldThrow<TransactionException>()
                                     .WithInnerMessage("database table is locked\r\ndatabase table is locked");
            }
        }

        [Test]
        public void Rollback_successfully()
        {
            // arrange
            var obj1 = new EntityClass1 { Item3 = "foo" };
            using (var uow = new UnitOfWork(SessionFactory))
            {
                var repo = new StandardRepository<EntityClass1>(uow);
                repo.SaveOrUpdate(obj1);
            }

            // act
            obj1.Item3 = "update";
            _repo.SaveOrUpdate(obj1);
            _unitOfWork.Rollback();

            // assert
            using (var uow = new UnitOfWork(SessionFactory))
            {
                var repo = new StandardRepository<EntityClass1>(uow);
                var entity = repo.Get(obj1.Id);
                entity.Item3
                      .Should()
                      .Be("foo");
            }
        }

        #endregion
    }
}