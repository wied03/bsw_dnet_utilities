// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

#region

using System;
using System.Linq;
using System.Linq.Expressions;

#endregion

// Credit to Gabriel Schenker (gnschenker@gmail.com)

#endregion

namespace Bsw.BaseEntities.Entity
{
    public abstract class StandardDbAssignedIntKeyEntity<TEntityType> : IStandardDbAssignedIntKeyEntity
        where TEntityType : StandardDbAssignedIntKeyEntity<TEntityType>
    {
        int? _oldHashCode;

// ReSharper disable once UnusedAutoPropertyAccessor.Global
        // NHibernate sets this
        public virtual int? Id { get; protected set; }

// ReSharper disable once MemberCanBePrivate.Global
        // only entities should need to access this
        protected bool Transient
        {
            get { return !Id.HasValue; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as TEntityType;
            if (other == null)
            {
                return false;
            }

            if (other.Transient && Transient)
            {
                return ReferenceEquals(other,
                                       this);
            }
            return other.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
// ReSharper disable NonReadonlyFieldInGetHashCode
            // need to be able to change this based on whether the entity is transient
            if (_oldHashCode.HasValue)
            {
                return _oldHashCode.Value;
            }

            if (!Transient) return Id.GetHashCode();

// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            // this is on purpose
            _oldHashCode = base.GetHashCode();
            return _oldHashCode.Value;
// ReSharper restore NonReadonlyFieldInGetHashCode
        }

        public static bool operator ==(StandardDbAssignedIntKeyEntity<TEntityType> x,
                                       StandardDbAssignedIntKeyEntity<TEntityType> y)
        {
            return Equals(x,
                          y);
        }

        public static bool operator !=(StandardDbAssignedIntKeyEntity<TEntityType> x,
                                       StandardDbAssignedIntKeyEntity<TEntityType> y)
        {
            return !(x == y);
        }
    }
}