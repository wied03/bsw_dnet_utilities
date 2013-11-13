// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;

namespace Bsw.Wpf.Utilities.Commands
{
    /// <summary>
    ///     More test friendly command wrapper
    /// </summary>
    public class AsyncDelegateCommandWrapper : DelegateCommand
    {
        private readonly Func<Task> _executeMethod;

        public AsyncDelegateCommandWrapper(Func<Task> executeMethod)
            : this(executeMethod,
                   () => true)
        {
        }

        public AsyncDelegateCommandWrapper(Func<Task> executeMethod,
                                           Func<bool> canExecuteMethod)
            : base(() => executeMethod(),
                   canExecuteMethod)
        {
            _executeMethod = executeMethod;
        }

        public virtual async Task ExecuteAsync()
        {
            await _executeMethod();
        }
    }
}