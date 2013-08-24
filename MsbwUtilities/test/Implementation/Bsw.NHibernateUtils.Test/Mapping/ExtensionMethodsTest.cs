#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Bsw.NHibernateUtils.Mapping;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NUnit.Framework;
using Rhino.Mocks;

#endregion

namespace Bsw.NHibernateUtils.Test.Mapping
{
    [TestFixture]
    public class ExtensionMethodsTest : BaseTest
    {
        private AutoPersistenceModel _persistenceModel;
        private IIdentityInstance _mockIdInstance;

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _persistenceModel = AutoMap.AssemblyOf<ExtensionMethodsTest>();
            _mockIdInstance = MockRepository.GenerateMock<IIdentityInstance>();
        }

        #endregion

        #region Utility Methods

        private List<IIdConvention> ActualIdConventions
        {
            get
            {
                return _persistenceModel
                    .Conventions
                    .Find<IIdConvention>()
                    .ToList();
            }
        }

        private IGeneratorInstance GeneratorMock
        {
            get
            {
                var generator = MockRepository.GenerateMock<IGeneratorInstance>();
                _mockIdInstance.Stub(id => id.GeneratedBy).Return(generator);
                return generator;
            }
        }

        #endregion

        #region Tests

        [Test]
        public void Support_nullable_primary_keys_native_generated()
        {
            // arrange
            var generator = GeneratorMock;

            // act
            _persistenceModel.SupportNullablePrimaryKeysNativeGenerated();
            ActualIdConventions
                .ForEach(idconv => idconv.Apply(_mockIdInstance));

            // assert
            generator.AssertWasCalled(g => g.Native());
        }

        [Test]
        public void Support_nullable_primary_keys_custom()
        {
            // arrange
            Action<IGeneratorInstance> sequenceGenerated = g => g.Sequence("theSequence");
            var generator = GeneratorMock;

            // act
            _persistenceModel.SupportNullablePrimaryKeys(sequenceGenerated);
            ActualIdConventions
                .ForEach(idconv => idconv.Apply(_mockIdInstance));

            // assert
            generator.AssertWasCalled(g => g.Sequence("theSequence"));
        }

        #endregion
    }
}