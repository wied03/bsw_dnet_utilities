// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
﻿using System;
using NHibernate;

namespace Bsw.NHibernateUtils.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        ISession CurrentSession { get; }
        void Commit(bool openNewTransactionAfterCommittingCurrent = true);
        void Rollback();
        bool Disposed { get; }
    }
}