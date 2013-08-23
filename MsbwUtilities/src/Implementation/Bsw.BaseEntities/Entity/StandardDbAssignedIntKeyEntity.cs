#region

using System;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace Bsw.BaseEntities.Entity
{
    public abstract class StandardDbAssignedIntKeyEntity
    {
        // for NHibernate
        protected StandardDbAssignedIntKeyEntity()
        {
        }

        public virtual int? Id { get; protected set; }
    }
}