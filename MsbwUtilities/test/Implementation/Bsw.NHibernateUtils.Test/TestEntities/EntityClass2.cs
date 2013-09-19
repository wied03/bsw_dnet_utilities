// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Linq;
using System.Linq.Expressions;
using Bsw.BaseEntities.Entity;

#endregion

namespace Bsw.NHibernateUtils.Test.TestEntities
{
    public class EntityClass2 : StandardDbAssignedIntKeyEntity<EntityClass2>
    {
        public virtual string Item1 { get; set; }
        public virtual string Item2 { get; set; }
    }
}