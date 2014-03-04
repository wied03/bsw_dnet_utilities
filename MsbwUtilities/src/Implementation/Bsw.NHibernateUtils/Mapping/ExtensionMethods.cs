// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Linq;
using System.Linq.Expressions;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions.Instances;

#endregion

namespace Bsw.NHibernateUtils.Mapping
{
    public static class ExtensionMethods
    {
        public static AutoPersistenceModel SupportNullablePrimaryKeysNativeGenerated(this AutoPersistenceModel model)
        {
            return model.SupportNullablePrimaryKeys(g => g.Native());
        }

        public static AutoPersistenceModel SupportNullablePrimaryKeys(this AutoPersistenceModel model,
                                                                      Action<IGeneratorInstance> generateStyle)
        {
            model.Conventions.Add(new NullablePrimaryKeyConvention(generateStyle));
            return model;
        }
    }
}