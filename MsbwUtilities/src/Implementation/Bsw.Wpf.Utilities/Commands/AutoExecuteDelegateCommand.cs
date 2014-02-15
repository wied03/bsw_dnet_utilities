// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace Bsw.Wpf.Utilities.Commands
{
    public class AutoExecuteDelegateCommand : ICommand
    {
        readonly DelegateCommand _wrapped;

        public AutoExecuteDelegateCommand(Action executeMethod)
            : this(executeMethod,
                   () => true)
        {
        }

        public AutoExecuteDelegateCommand(Action executeMethod,
                                          Func<bool> canExecuteMethod)
        {
            _wrapped = new DelegateCommand(executeMethod,
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