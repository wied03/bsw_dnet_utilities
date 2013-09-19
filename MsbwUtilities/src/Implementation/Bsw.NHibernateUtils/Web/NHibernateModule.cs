// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

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
        private ILazySessionFetcher _lazySessionFetcher;

        public void Init(HttpApplication context)
        {
            context.BeginRequest += ContextBeginRequest;
            context.EndRequest += ContextEndRequest;
        }

        private void ContextBeginRequest(object sender,
                                         EventArgs e)
        {
            _lazySessionFetcher = ObjectFactory.GetInstance<ILazySessionFetcher>();
        }

        private void ContextEndRequest(object sender,
                                       EventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_lazySessionFetcher == null)
                return;

            _lazySessionFetcher.Dispose();
            _lazySessionFetcher = null;
        }
    }
}