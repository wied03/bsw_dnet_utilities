// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
﻿#region

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Bsw.NHibernateUtils.Repository;
using StructureMap;

#endregion

namespace Bsw.NHibernateUtils.Web
{
    public class NHibernateModule : IHttpModule
    {
        private IUnitOfWork _unitOfWork;

        public void Init(HttpApplication context)
        {
            context.BeginRequest += ContextBeginRequest;
            context.EndRequest += ContextEndRequest;
        }

        private void ContextBeginRequest(object sender,
                                         EventArgs e)
        {
            _unitOfWork = ObjectFactory.GetInstance<IUnitOfWork>();
        }

        private void ContextEndRequest(object sender,
                                       EventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_unitOfWork == null)
                return;

            _unitOfWork.Dispose();
            _unitOfWork = null;
        }
    }
}