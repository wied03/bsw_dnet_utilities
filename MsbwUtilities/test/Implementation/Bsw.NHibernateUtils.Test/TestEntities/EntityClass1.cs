#region

using System;
using System.Linq;
using System.Linq.Expressions;
using Bsw.BaseEntities.Entity;

#endregion

namespace Bsw.NHibernateUtils.Test.TestEntities
{
    public class EntityClass1
    {
        public virtual int? Id { get; set; }
        public virtual string Item3 { get; set; }
        public virtual string Item4 { get; set; }
    }
}