// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Linq;
using System.Linq.Expressions;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

#endregion

namespace Bsw.NHibernateUtils.Mapping
{
    public class NullablePrimaryKeyConvention : IIdConvention
    {
        private readonly Action<IGeneratorInstance> _generateStyle;

        public NullablePrimaryKeyConvention(Action<IGeneratorInstance> generateStyle)
        {
            _generateStyle = generateStyle;
        }

        public void Apply(IIdentityInstance instance)
        {
            _generateStyle(instance.GeneratedBy);
        }
    }
}