// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Linq;
using System.Linq.Expressions;
using Bsw.NHibernateUtils.Repository;
using Bsw.NHibernateUtils.Test.TestEntities;
using FluentAssertions;
using NHibernate.Linq;
using NUnit.Framework;

#endregion

namespace Bsw.NHibernateUtils.Test.Repository
{
    [TestFixture]
    public class LazySessionFetcherTest : BaseDbDrivenTest
    {
        private ILazySessionFetcher _lazySessionFetcher;

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _lazySessionFetcher = new LazySessionFetcher(SessionFactory);
        }

        [TearDown]
        public override void TearDown()
        {
            _lazySessionFetcher.Dispose();
            base.TearDown();
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
            _lazySessionFetcher.Dispose();

            // assert
            SessionFactory.Statistics.SessionOpenCount.Should().Be(0);
        }

        [Test]
        public void Fetch_session()
        {
            using (var session = _lazySessionFetcher.Session)
            {
                var obj1 = new EntityClass1 {Item4 = "Explicit_commit_normal"};
                using (var transaction = session.BeginTransaction())
                {
                    // arrange
                    session.SaveOrUpdate(obj1);
                    // act
                    transaction.Commit();
                    // assert
                    obj1.Id
                        .Should()
                        .HaveValue();
                }
                using (session.BeginTransaction())
                {
                    var entity = session.Query<EntityClass1>()
                                        .First(e => e.Item4 == "Explicit_commit_normal");
                    entity.ShouldBeEquivalentTo(obj1);
                }
            }
        }

        #endregion
    }
}