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