// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace Bsw.Wpf.Utilities.Commands
{
    public class VirtualFriendlyDelegateCommand<T> : ICommand
    {
        private readonly DelegateCommand<T> _command;

        public VirtualFriendlyDelegateCommand(Action<T> executeCommand)
            : this(executeCommand,
                   p => true)
        {
        }

        public VirtualFriendlyDelegateCommand(Action<T> executeCommand,
                                              Func<T, bool> canExecuteMethod)
        {
            _command = new DelegateCommand<T>(executeCommand,
                                              canExecuteMethod);
        }

        public bool CanExecute(object parameter)
        {
            return CanExecute((T) parameter);
        }

        public virtual bool CanExecute(T parameter)
        {
            return _command.CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            Execute((T) parameter);
        }

        public virtual void Execute(T parameter)
        {
            _command.Execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { _command.CanExecuteChanged += value; }
            remove { _command.CanExecuteChanged -= value; }
        }
    }
}