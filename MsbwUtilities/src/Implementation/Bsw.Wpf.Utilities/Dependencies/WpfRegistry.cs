// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bsw.Wpf.Utilities.Services;
using Microsoft.Practices.Prism.Events;
using StructureMap.Configuration.DSL;

namespace Bsw.Wpf.Utilities.Dependencies
{
    public class WpfRegistry : Registry
    {
        public WpfRegistry()
        {
            Scan(s =>
                 {
                     s.AssemblyContainingType<WpfRegistry>();
                     s.WithDefaultConventions();
                 });
            For<TaskScheduler>()
                .Use(TaskScheduler.FromCurrentSynchronizationContext);
            For<IFetchCurrentWindow>()
                .Singleton();
            For<IEventAggregator>()
                .Singleton() // Need the same one for events to route properly
                .Use<EventAggregator>();
        }
    }
}