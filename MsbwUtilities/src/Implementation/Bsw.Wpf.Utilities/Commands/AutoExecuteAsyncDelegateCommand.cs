// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bsw.Coworking.Agent.Config.Utilities.Commands
{
    public class AutoExecuteAsyncDelegateCommand : ICommand
    {
        private readonly AsyncDelegateCommandWrapper _wrapped;

        public AutoExecuteAsyncDelegateCommand(Func<Task> executeMethod)
            : this(executeMethod,
                   () => true)
        {
        }

        public AutoExecuteAsyncDelegateCommand(Func<Task> executeMethod,
                                               Func<bool> canExecuteMethod)
        {
            _wrapped = new AsyncDelegateCommandWrapper(executeMethod,
                                                       canExecuteMethod);
        }

        public bool CanExecute(object parameter = null)
        {
            return _wrapped.CanExecute();
        }

        public void Execute(object parameter = null)
        {
            _wrapped.Execute();
        }

        public async Task ExecuteAsync()
        {
            await _wrapped.ExecuteAsync();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                _wrapped.CanExecuteChanged += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                _wrapped.CanExecuteChanged -= value;
            }
        }
    }
}